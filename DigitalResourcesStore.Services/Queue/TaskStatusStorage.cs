using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalResourcesStore.Services.Queue
{
    public static class TaskStatusStorage
    {
        private static readonly Dictionary<Guid, string> _taskStatuses = new Dictionary<Guid, string>();
        private static readonly Dictionary<int, Guid> _userTasks = new Dictionary<int, Guid>();

        public static void SetTaskStatus(Guid taskId, string status, int? userId = null)
        {
            lock (_taskStatuses)
            {
                _taskStatuses[taskId] = status;
                if (userId.HasValue)
                {
                    _userTasks[userId.Value] = taskId;
                }
            }
        }

        public static string GetTaskStatus(Guid taskId)
        {
            lock (_taskStatuses)
            {
                return _taskStatuses.ContainsKey(taskId) ? _taskStatuses[taskId] : null;
            }
        }

        public static bool IsTaskInProgressForUser(int userId)
        {
            lock (_userTasks)
            {
                if (_userTasks.TryGetValue(userId, out var taskId))
                {
                    return GetTaskStatus(taskId) == "Processing";
                }
                return false;
            }
        }
    }
}

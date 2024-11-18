namespace DigitalResourcesStore.Models
{
	/// <summary>
	/// PageRequest là định nghĩa cho một object truy vấn phân trang cho 1 đối tượng
	/// </summary>
	public class PagedRequest
	{
		/// <summary>
		/// PageIndex là định nghĩa cho số trang
		/// </summary>
		public int PageIndex { get; set; } = 1;
		/// <summary>
		/// PageSize là định nghĩa cho số dòng trong trang
		/// </summary>
		public int PageSize { get; set; } = 10;
	}
	/// <summary>
	/// PagedResponse là định nghĩa kết quả sau khi truy vấn (đã phân trang) cho 1 đối tượng
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PagedResponse<T>
	{
		public int PageIndex { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public List<T> Data { get; set; }
		public long TotalCount { get; set; } = 0;
		public int TotalPage { get; set; } = 0;
		public PagedResponse(ICollection<T> data, int pageIndex, int pageSize, long totalCount)
		{
			PageIndex = pageIndex;
			PageSize = pageSize;
			Data = data.ToList();
			TotalCount = totalCount;
			TotalPage = (int)Math.Ceiling((double)totalCount / pageSize);
		}
	}
	public static class PagedHandler<T>
	{
		public static IQueryable<T> Page(IQueryable<T> data, int pageIndex, int pageSize)
		{
			return data.Skip((pageIndex - 1) * pageSize).Take(pageSize);
		}
		public static IQueryable<T> Page(IQueryable<T> data, PagedRequest paging)
		{
			return data.Skip((paging.PageIndex - 1) * paging.PageSize).Take(paging.PageSize);
		}
		public static IQueryable<T> Page(IOrderedQueryable<T> data, int pageIndex, int pageSize)
		{
			return data.Skip((pageIndex - 1) * pageSize).Take(pageSize);
		}
		public static IQueryable<T> Page(IOrderedQueryable<T> data, PagedRequest paging)
		{
			return data.Skip((paging.PageIndex - 1) * paging.PageSize).Take(paging.PageSize);
		}
	}
}

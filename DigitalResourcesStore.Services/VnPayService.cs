using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalResourcesStore.Models.DepositDtos;
using DigitalResourcesStore.Models.CartsDtos;

namespace DigitalResourcesStore.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collection);
        bool ValidateSignature(string secureHash, IQueryCollection collection);
    }
    public interface IVnPayCartService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentCartRequestModel model);
        VnPaymentCartResponseModel PaymentExecute(IQueryCollection collection);
    }

    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", model.Id);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", $"{model.Id}_{tick}");

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collection)
        {
            var vnpay = new VnPayLibrary();
            foreach (var item in collection)
            {
                if (!string.IsNullOrEmpty(item.Key) && item.Key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(item.Key, item.Value.ToString());
                }
            }

            var vnp_orderId = vnpay.GetResponseData("vnp_TxnRef").ToString();
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_Amount = vnpay.GetResponseData("vnp_Amount");

            // Ensure vnp_Amount is a valid numeric value before conversion
            if (long.TryParse(vnp_Amount.ToString(), out long amount))
            {
                amount /= 100; // Amount is in the smallest unit, so divide by 100
            }
            else
            {
                return new VnPaymentResponseModel
                {
                    Success = false,
                    ErrorMessage = "Invalid amount format"
                };
            }

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VNPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_orderId.ToString(),
                OrderId = vnp_OrderInfo,
                Amount = amount, // Corrected value
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayReponseCode = vnp_ResponseCode.ToString(),
            };
        }


        public bool ValidateSignature(string secureHash, IQueryCollection collection)
        {
            var vnpay = new VnPayLibrary();
            foreach (var item in collection)
            {
                if (!string.IsNullOrEmpty(item.Key) && item.Key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(item.Key, item.Value.ToString());
                }
            }

            return vnpay.ValidateSignature(secureHash, _config["VNPay:HashSecret"]);
        }
    }

    public class VnPayCartService : IVnPayCartService
    {
        private readonly IConfiguration _config;

        public VnPayCartService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext context, VnPaymentCartRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", _config["VnPayCart:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPayCart:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPayCart:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", model.Amount.ToString());
            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPayCart:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPayCart:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", model.Id);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPayCart:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", $"{model.Id}_{tick}");

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPayCart:BaseUrl"], _config["VnPayCart:HashSecret"]);
            return paymentUrl;
        }

        public VnPaymentCartResponseModel PaymentExecute(IQueryCollection collection)
        {
            var vnpay = new VnPayLibrary();
            foreach (var item in collection)
            {
                if (!string.IsNullOrEmpty(item.Key) && item.Key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(item.Key, item.Value.ToString());
                }
            }

            var vnp_orderId = vnpay.GetResponseData("vnp_TxnRef").ToString();
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var vnp_Amount = vnpay.GetResponseData("vnp_Amount");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VNPayCart:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentCartResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentCartResponseModel
            {
                Success = true,
                PaymentMethod = "VnPayCart",
                OrderDescription = vnp_orderId.ToString(),
                OrderId = vnp_OrderInfo,
                Amount = Convert.ToInt32(vnp_Amount) / 100,
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayReponseCode = vnp_ResponseCode.ToString(),
            };
        }
    }
}

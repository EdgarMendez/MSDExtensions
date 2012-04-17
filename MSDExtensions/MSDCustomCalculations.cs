using System;
using System.Collections.Generic;
using System.Text;

using Ektron.Cms.Instrumentation;
using Ektron.Cms.Extensibility;
using Ektron.Cms.Extensibility.Commerce;
using Ektron.Cms.Commerce;
using Ektron.Cms.Common;

using System.Net.Mail;

namespace Ektron.Cms.Extensibility.Commerce.MSD
{
    
    public class MSDBasketCalculation : Ektron.Cms.Extensibility.Commerce.BasketCalculatorStrategy
    {
        public override void OnAfterCalculate(BasketCalculatorData basketCalculatorData, CmsEventArgs eventArgs)
        {
            //Apply tax rate to shipping
            decimal basketSubtotal = basketCalculatorData.Basket.Subtotal;
            decimal basketTotalTaxes = basketCalculatorData.TotalTaxes;
            decimal basketTotalShippingCost = basketCalculatorData.TotalShippingCost;
            decimal taxRate = 0;
            decimal shipTax = 0;
            decimal totalTaxes = 0;

            if (basketSubtotal != 0) {
                taxRate = basketTotalTaxes / basketSubtotal;
                shipTax =  basketTotalShippingCost * taxRate;
                totalTaxes = basketTotalTaxes + shipTax;
                basketCalculatorData.TotalTaxes = totalTaxes; 
            }
        }
    }

    public class MSDOrderManagement : Ektron.Cms.Extensibility.Commerce.OrderStrategy
    {
        public override void OnAfterOrderPlaced(OrderData orderData, CmsEventArgs eventArgs)
        {
            String ToEmail = "ecommerce@msdignition.com";
            String FromAddress = "registration@msdignition.com";
            String BodyMessage = String.Format("{2} {3} has placed Order #{0} on {1}", orderData.Id.ToString(),orderData.DateCreated.ToShortDateString(),orderData.Customer.FirstName,orderData.Customer.LastName);
            String Subject = String.Format("Order #{0} has been placed on MSD Powersports", orderData.Id);
                       
            Ektron.Cms.Extensibility.Commerce.MSD.MSDHelperUtilities.SendOrderMail(FromAddress, ToEmail, Subject, BodyMessage);
        }
    }
    
    public class MSDHelperUtilities
    {
        public static void SendOrderMail(String FromAddress, String ToAddress, String Subject, String Body)
        {
            MailMessage Msg = new MailMessage(FromAddress, ToAddress, Subject, Body);
            SmtpClient smtp = new SmtpClient("localhost");
            smtp.Send(Msg);
        }
    
    }
    
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;

using System.Configuration.Provider;
using Ektron.Cms.Commerce.Shipment.Provider;
using Ektron.Cms.Commerce;
using Ektron.Cms.Instrumentation;
using Ektron.Cms.Extensibility.Commerce;



namespace Ektron.Cms.Extensibility.Commerce.MSD
{

    /// <summary>
    /// MSD Shipping Rate implementation of Ektron Cms 400.Net eCommerce Shipment Provider.
    /// </summary>
    public class MSDShipmentProvider : ShipmentProvider
    {

        #region constructor, member variables
        public MSDShipmentProvider()
        {
            IsTrackingSupported = false;

            _shippingOptionList = new List<string>();
            _shippingOptionList.Add("MSD_Ground"); 
            _shippingOptionList.Add("MSD_3_Day");
            _shippingOptionList.Add("MSD_2_Day");
            _shippingOptionList.Add("MSD_1_Day");
            _shippingOptionList.Add("MSD_Free_Shipping");

        }


        private List<string> _shippingOptionList;

        #endregion


        #region ShipmentProvider Implementation
        /// <summary>
        /// Provider intitialization.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {

            if (config == null)
                throw new ArgumentNullException("config");

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
                name = "MSDShipmentProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "MSDShipmentProvider Provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            // Throw an exception if unrecognized attributes remain
            if (config.Count == 0)
            {
                throw new ProviderException("Shipment provider attribute missing.");
            }
            else
            {
                //read all config attributes.
                ServiceUrl = config["serviceUrl"];
                Key = config["key"];
                Password = config["password"];
                AccountNumber = config["accountNumber"];
                MeterNumber = config["meterNumber"];
                TransactionId = config["transactionId"];
            }
        }

        /// <summary>
        /// Returns list of supported Shipping options
        /// </summary>
        /// <returns></returns>
        public override List<string> GetServiceTypes()
        {
            return _shippingOptionList;
        }

        /// <summary>
        /// Returns tracking url for tracking Id number (not supported).
        /// </summary>
        /// <param name="trackingId"></param>
        /// <returns></returns>
        public override string GetTrackingUrl(string trackingId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<a href='http://www.fedex.com/Tracking?tracknumbers=");
            sb.Append(trackingId);
            sb.Append("&action=track'>");
            sb.Append(trackingId);
            sb.Append("</a>");

            return (sb.ToString());
        }


        /// <summary>
        /// Returns list of rates base dupon the supplied information.
        /// </summary>
        /// <param name="expectedOptions">Shipping Options for which to receive rates.</param>
        /// <param name="origin">from address.</param>
        /// <param name="destination">destination address</param>
        /// <param name="weight">Weight of package</param>
        /// <param name="dimensions">Diemnsions of Package</param>
        /// <returns></returns>
        public override List<ShippingOptionData> GetRates(IEnumerable<ShippingMethodData> expectedOptions, AddressData origin, AddressData destination, Weight weight, Dimensions dimensions)
        {

            List<ShippingOptionData> availableOptions = new List<ShippingOptionData>();

            try
            {
                foreach (ShippingMethodData expectedOption in expectedOptions)
                {
                    Log.WriteInfo("MSD Shipping Provider.  ExpectedOption:" + expectedOption.Name);
                    
                    decimal cartTotal = 0M;
                    Ektron.Cms.Commerce.BasketApi bapi = new BasketApi();
                    Basket myBasket = bapi.GetDefaultBasket();
                    cartTotal = myBasket.Subtotal;

                    switch (expectedOption.ProviderService.ToLower())
                    {
                        case "msd_ground":

                            ShippingOptionData groundShipOption = new ShippingOptionData();
                            groundShipOption.Id = expectedOption.Id;
                            groundShipOption.Name = expectedOption.Name;

                            if (cartTotal <= 99.00M)
                            {
                                groundShipOption.ShippingFee = 10.00M;
                            }
                            else if (cartTotal > 99.00M && cartTotal <= 299.00M)
                            {
                                groundShipOption.ShippingFee = 15.00M;
                            }
                            else if (cartTotal > 299.00M && cartTotal <= 799.00M)
                            {
                                groundShipOption.ShippingFee = 20.00M;
                            }

                            else if (cartTotal > 799.00M && cartTotal <= 999.00M)
                            {
                                groundShipOption.ShippingFee = 25.00M;
                            }
                            else if (cartTotal > 999.00M && cartTotal <= 10000.00M)
                            {
                                groundShipOption.ShippingFee = 100.00M;
                            }
                            else if (cartTotal > 10000.00M)
                            {
                                groundShipOption.ShippingFee = 300.00M;
                            }
                            groundShipOption.ProviderService = "MSD_Ground";
                            availableOptions.Add(groundShipOption);
                            break;

                        case "msd_3_day":

                            ShippingOptionData ThreeDayShipOption = new ShippingOptionData();
                            ThreeDayShipOption.Id = expectedOption.Id;
                            ThreeDayShipOption.Name = expectedOption.Name;

                            if (cartTotal <= 99.00M)
                            {
                                ThreeDayShipOption.ShippingFee = 15.00M;
                            }
                            else if (cartTotal > 99.00M && cartTotal <= 299.00M)
                            {
                                ThreeDayShipOption.ShippingFee = 20.00M;
                            }
                            else if (cartTotal > 299.00M && cartTotal <= 799.00M)
                            {
                                ThreeDayShipOption.ShippingFee = 30.00M;
                            }

                            else if (cartTotal > 799.00M && cartTotal <= 999.00M)
                            {
                                ThreeDayShipOption.ShippingFee = 35.00M;
                            }
                            else if (cartTotal > 999.00M && cartTotal <= 10000.00M)
                            {
                                ThreeDayShipOption.ShippingFee = 125.00M;
                            }
                            else if (cartTotal > 10000.00M)
                            {
                                ThreeDayShipOption.ShippingFee = 350.00M;
                            }


                            ThreeDayShipOption.ProviderService = "MSD_3_Day";
                            availableOptions.Add(ThreeDayShipOption);
                            break;


                        case "msd_2_day":

                            ShippingOptionData TwoDayShipOption = new ShippingOptionData();
                            TwoDayShipOption.Id = expectedOption.Id;
                            TwoDayShipOption.Name = expectedOption.Name;

                            if (cartTotal <= 99.00M)
                            {
                                TwoDayShipOption.ShippingFee = 20.00M;
                            }
                            else if (cartTotal > 99.00M && cartTotal <= 299.00M)
                            {
                                TwoDayShipOption.ShippingFee = 25.00M;
                            }
                            else if (cartTotal > 299.00M && cartTotal <= 799.00M)
                            {
                                TwoDayShipOption.ShippingFee = 40.00M;
                            }

                            else if (cartTotal > 799.00M && cartTotal <= 999.00M)
                            {
                                TwoDayShipOption.ShippingFee = 45.00M;
                            }
                            else if (cartTotal > 999.00M && cartTotal <= 10000.00M)
                            {
                                TwoDayShipOption.ShippingFee = 150.00M;
                            }
                            else if (cartTotal > 10000.00M)
                            {
                                TwoDayShipOption.ShippingFee = 400.00M;
                            }
                            
                            TwoDayShipOption.ProviderService = "MSD_2_Day";
                            availableOptions.Add(TwoDayShipOption);
                            break;

                            case "msd_1_day":

                            ShippingOptionData OneDayShipOption = new ShippingOptionData();
                            OneDayShipOption.Id = expectedOption.Id;
                            OneDayShipOption.Name = expectedOption.Name;

                            if (cartTotal <= 99.00M)
                            {
                                OneDayShipOption.ShippingFee = 25.00M;
                            }
                            else if (cartTotal > 99.00M && cartTotal <= 299.00M)
                            {
                                OneDayShipOption.ShippingFee = 30.00M;
                            }
                            else if (cartTotal > 299.00M && cartTotal <= 799.00M)
                            {
                                OneDayShipOption.ShippingFee = 50.00M;
                            }

                            else if (cartTotal > 799.00M && cartTotal <= 999.00M)
                            {
                                OneDayShipOption.ShippingFee = 55.00M;
                            }
                            else if (cartTotal > 999.00M && cartTotal <= 10000.00M)
                            {
                                OneDayShipOption.ShippingFee = 175.00M;
                            }
                            else if (cartTotal > 10000.00M)
                            {
                                OneDayShipOption.ShippingFee = 450.00M;
                            }
                            
                            OneDayShipOption.ProviderService = "MSD_1_Day";
                            availableOptions.Add(OneDayShipOption);
                            break;

                            case "msd_free_shipping":

                            ShippingOptionData FreeShipOption = new ShippingOptionData();
                            FreeShipOption.Id = expectedOption.Id;
                            FreeShipOption.Name = expectedOption.Name;

                            FreeShipOption.ShippingFee = 0.00M;

                            FreeShipOption.ProviderService = "MSD_Free_Shipping";
                            availableOptions.Add(FreeShipOption);
                            break;
                    
                    }

                }

            }
            catch (Exception e)
            {
                Log.WriteError("MSD Shipping Provider: Error retrieving shipping rates." + e.Message);
                throw;
            }
            return availableOptions;
        }

        #endregion

    }
}
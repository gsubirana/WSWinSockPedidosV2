using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comuns
{
    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Order
    {

        private OrderOrderHeader orderHeaderField;

        private OrderOrderItem[] orderDetailField;

        /// <comentarios/>
        public OrderOrderHeader OrderHeader
        {
            get
            {
                return this.orderHeaderField;
            }
            set
            {
                this.orderHeaderField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlArrayItemAttribute("OrderItem", IsNullable = false)]
        public OrderOrderItem[] OrderDetail
        {
            get
            {
                return this.orderDetailField;
            }
            set
            {
                this.orderDetailField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OrderOrderHeader
    {

        private string documentTypeField;

        private string orderNumberField;

        private string salesOrganizationField;

        private string distrChannelField;

        private string paymentTermsField;

        private OrderOrderHeaderDates datesField;

        private OrderOrderHeaderPartner[] partnersField;

        /// <comentarios/>
        public string DocumentType
        {
            get
            {
                return this.documentTypeField;
            }
            set
            {
                this.documentTypeField = value;
            }
        }

        /// <comentarios/>
        public string OrderNumber
        {
            get
            {
                return this.orderNumberField;
            }
            set
            {
                this.orderNumberField = value;
            }
        }

        /// <comentarios/>
        public string SalesOrganization
        {
            get
            {
                return this.salesOrganizationField;
            }
            set
            {
                this.salesOrganizationField = value;
            }
        }

        /// <comentarios/>
        public string DistrChannel
        {
            get
            {
                return this.distrChannelField;
            }
            set
            {
                this.distrChannelField = value;
            }
        }


        public string PaymentTerms
        {
            get
            {
                return this.paymentTermsField;
            }
            set
            {
                this.paymentTermsField = value;
            }
        }



        /// <comentarios/>
        public OrderOrderHeaderDates Dates
        {
            get
            {
                return this.datesField;
            }
            set
            {
                this.datesField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Partner", IsNullable = false)]
        public OrderOrderHeaderPartner[] Partners
        {
            get
            {
                return this.partnersField;
            }
            set
            {
                this.partnersField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OrderOrderHeaderDates
    {

        private string orderDateField;

        private string requestedDeliveryDateField;

        /// <comentarios/>
        public string OrderDate
        {
            get
            {
                return this.orderDateField;
            }
            set
            {
                this.orderDateField = value;
            }
        }

        /// <comentarios/>
        public string RequestedDeliveryDate
        {
            get
            {
                return this.requestedDeliveryDateField;
            }
            set
            {
                this.requestedDeliveryDateField = value;
            }
        }
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OrderOrderHeaderPartner
    {

        private string partnerTypeField;

        private string partnerIDField;

        //private string partnerIDFieldSpecified;

        //private string partnerCodeField;

        //private string partnerCodeFieldSpecified;

        /// <comentarios/>
        public string PartnerType
        {
            get
            {
                return this.partnerTypeField;
            }
            set
            {
                this.partnerTypeField = value;
            }
        }

        /// <comentarios/>
        public string PartnerID
        {
            get
            {
                return this.partnerIDField;
            }
            set
            {
                this.partnerIDField = value;
            }
        }

        ///// <comentarios/>
        //[System.Xml.Serialization.XmlIgnoreAttribute()]
        //public string PartnerIDSpecified
        //{
        //    get
        //    {
        //        return this.partnerIDFieldSpecified;
        //    }
        //    set
        //    {
        //        this.partnerIDFieldSpecified = value;
        //    }
        //}

        ///// <comentarios/>
        //public string PartnerCode
        //{
        //    get
        //    {
        //        return this.partnerCodeField;
        //    }
        //    set
        //    {
        //        this.partnerCodeField = value;
        //    }
        //}

        ///// <comentarios/>
        //[System.Xml.Serialization.XmlIgnoreAttribute()]
        //public string PartnerCodeSpecified
        //{
        //    get
        //    {
        //        return this.partnerCodeFieldSpecified;
        //    }
        //    set
        //    {
        //        this.partnerCodeFieldSpecified = value;
        //    }
        //}
    }

    /// <comentarios/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class OrderOrderItem
    {

        private string lineItemNumberField;

        private string productCodeField;

        private string orderQtyField;

        /// <comentarios/>
        public string LineItemNumber
        {
            get
            {
                return this.lineItemNumberField;
            }
            set
            {
                this.lineItemNumberField = value;
            }
        }

        /// <comentarios/>
        public string ProductCode
        {
            get
            {
                return this.productCodeField;
            }
            set
            {
                this.productCodeField = value;
            }
        }

        /// <comentarios/>
        public string OrderQty
        {
            get
            {
                return this.orderQtyField;
            }
            set
            {
                this.orderQtyField = value;
            }
        }
    }


}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Web.Mvc;

namespace IEEEConference.Models
{
    public class CommonDropList
    {
        [Display(Name = "Item Number")]
        public int ID { get; set; }

        [StringLength(50)]
        [Display(Name = "Category")]
        public string Category { get; set; }
        [Display(Name = "Sequence Number")]
        public int SequenceNum { get; set; }
        [StringLength(50)]
        [Display(Name = "Text")]
        public string Text { get; set; }
        [StringLength(50)]
        [Display(Name = "Value")]
        public string Value { get; set; }
    }

    public class UserInformation
    {
        //  **  Primary Key **  //
        [Display(Name = "ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [StringLength(128)]
        [Display(Name = "User Account")]
        public string UserAccount { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [StringLength(8)]
        [Display(Name = "Title")]
        public String TitleName { get; set; }
        [StringLength(64)]
        [Display(Name = "Family Name")]
        public String FamilyName { get; set; }
        [StringLength(64)]
        [Display(Name = "Given Name")]
        public String GivenName { get; set; }
        [StringLength(64)]
        [Display(Name = "Nick Name")]
        public String NickName { get; set; }
        [StringLength(2000)]
        [Display(Name = "Affiliation")]
        public String Affiliation { get; set; }
        [StringLength(256)]
        [Display(Name = "Institution / Company")]
        public String Institution { get; set; }
        [StringLength(256)]
        [Display(Name = "Department / Unit")]
        public String Department { get; set; }
        [StringLength(64)]
        [Display(Name = "Contact Number")]
        public String Contact { get; set; }
        [StringLength(128)]
        [Display(Name = "Country")]
        public String Country { get; set; }
        [StringLength(64)]
        [Display(Name = "Permission Group")]
        public String PermissionGroup { get; set; }
        [StringLength(64)]
        [Display(Name = "Permission Enabled")]
        public String PermissionEnabled { get; set; }
        [Display(Name = "Select Conference")]
        public int SelectedConference { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }

        [Display(Name = "User Name")]
        public String UserName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(FamilyName)) { return ""; };
                return TitleName + " " + FamilyName + ", " + GivenName + " " + NickName;
            }
        }

    }

    public class Conference
    {
        //  **  Primary Key **  //
        [Display(Name = "Conference ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [StringLength(64)]
        [Display(Name = "Conference Code")]
        public String Code { get; set; }
        [StringLength(64)]
        [Display(Name = "Conference Category")]
        public String Category { get; set; }
        [StringLength(4)]
        [Display(Name = "Conference Year")]
        public String ConferenceYear { get; set; }
        [StringLength(128)]
        [Display(Name = "Title")]
        public String Title { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "From")]
        public System.Nullable<DateTime> PeriodFrom { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "To")]
        public System.Nullable<DateTime> PeriodTo { get; set; }
        [StringLength(128)]
        [Display(Name = "Venue")]
        public String Venue { get; set; }
        [StringLength(64)]
        [Display(Name = "City")]
        public String City { get; set; }
        [StringLength(128)]
        [Display(Name = "Country")]
        public String Country { get; set; }
        [StringLength(256)]
        [Display(Name = "Home page")]
        public String HomeUrl { get; set; }
        [StringLength(256)]
        [Display(Name = "Contact")]
        public String Contact { get; set; }
        [StringLength(64)]
        [Display(Name = "Style")]
        public String Style { get; set; }            //  Oral or Poster
        [StringLength(256)]
        [Display(Name = "Poster File")]
        public String FileNamePoster { get; set; }
        [StringLength(256)]
        [Display(Name = "Poster File Path")]
        public String FilePathPoster { get; set; }
        [StringLength(256)]
        [Display(Name = "Distributed Media")]
        public String FileNameMedia { get; set; }
        [StringLength(256)]
        [Display(Name = "Distributed Media File Path")]
        public String FilePathMedia { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }
        [StringLength(256)]
        [Display(Name = "Revoke Reason")]
        public String RevokeReason { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Close Date")]
        public System.Nullable<DateTime> DateClose { get; set; }
        [StringLength(64)]
        [Display(Name = "Close User")]
        public String UserClose { get; set; }

        //  **  Entity Relationship **  //
        public virtual ICollection<ControlDate> ControlDates { get; set; }
        public virtual ICollection<ConfigureVPC> ConfigureVPCs { get; set; }
        public virtual ICollection<PriceItem> PriceItems { get; set; }
        public virtual ICollection<Field> Fields { get; set; }
        public virtual ICollection<Committee> Committees { get; set; }
        public virtual ICollection<Paper> Papers { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }

    public class ControlDate
    {
        //  **  Primary Key **  //
        [Display(Name = "System ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [StringLength(64)]
        [Display(Name = "Category")]
        public String Category { get; set; }
        [StringLength(256)]
        [Display(Name = "Explanation")]
        public String Explanation { get; set; }
        [StringLength(64)]
        [Display(Name = "Description")]
        public String Description { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "From")]
        public System.Nullable<DateTime> PeriodFrom { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "To")]
        public System.Nullable<DateTime> PeriodTo { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }

        //  **  Foreign Key **  //
        //[Key]
        //[ForeignKey("ConferenceID")]
        public int ConferenceID { get; set; }
        //  **  Entity Relationship **  //
        public virtual Conference Conference { get; set; }
    }

    public class ConfigureVPC      //  Virtual Payment Client (VPC) on credit card
    {
        //  **  Primary Key **  //
        [Display(Name = "System ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [StringLength(64)]
        [Display(Name = "Unit")]
        public String Unit { get; set; }        //  Current Unit (USD, HKD, ...)
        [StringLength(64)]
        [Display(Name = "Title")]
        public String Title { get; set; }
        [StringLength(256)]
        [Display(Name = "Explanation")]
        public String Explanation { get; set; }
        [StringLength(256)]
        [Display(Name = "Name")]
        public String Name { get; set; }
        [StringLength(256)]
        [Display(Name = "Value")]
        public String Value { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }

        //  **  Foreign Key **  //
        //[Key]
        //[ForeignKey("ConferenceID")]
        public int ConferenceID { get; set; }
        //  **  Entity Relationship **  //
        public virtual Conference Conference { get; set; }
    }

    public class PriceItem
    {
        //  **  Primary Key **  //
        [Display(Name = "System ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [StringLength(64)]
        [Display(Name = "Category")]
        public String Category { get; set; }
        [StringLength(64)]
        [Display(Name = "Unit")]
        public String Unit { get; set; }        //  Current Unit (USD, HKD, ...)
        [StringLength(64)]
        [Display(Name = "Type")]
        public String Type { get; set; }        //  Description of Account, Description of payment item
        [StringLength(128)]
        [Display(Name = "Description")]
        public String Description { get; set; }
        [Display(Name = "Price")]
        public System.Nullable<Decimal> Price { get; set; }
        [StringLength(64)]
        [Display(Name = "Enabled VPC")]
        public String EnabledVPC { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }

        //  **  Foreign Key **  //
        //[Key]
        //[ForeignKey("ConferenceID")]
        public int ConferenceID { get; set; }
        //  **  Entity Relationship **  //
        public virtual Conference Conference { get; set; }
    }

    public class Field
    {
        //  **  Primary Key **  //
        [Display(Name = "System ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [Display(Name = "Row Number")]
        public int RowNum { get; set; }
        [StringLength(64)]
        [Display(Name = "Category")]
        public String Code { get; set; }
        [StringLength(64)]
        [Display(Name = "Description")]
        public String Description { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }

        //  **  Foreign Key **  //
        //[Key]
        //[ForeignKey("ConferenceID")]
        public int ConferenceID { get; set; }
        //  **  Entity Relationship **  //
        public virtual Conference Conference { get; set; }
    }

    public class Committee
    {
        //  **  Primary Key **  //
        [Display(Name = "System ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [StringLength(128)]
        [Display(Name = "User Account")]
        public string UserAccount { get; set; }
        [StringLength(64)]
        [Display(Name = "Authorization")]
        public string Authorization { get; set; }
        [StringLength(64)]
        [Display(Name = "Permission Group")]
        public String PermissionGroup { get; set; }     // Chair, TPC, Speaker, FieldCode
        [DataType(DataType.Date)]
        [Display(Name = "First Notice Date")]
        public System.Nullable<DateTime> FirstNoticeDate { get; set; }
        [StringLength(64)]
        [Display(Name = "First Notice By")]
        public String FirstNoticeUser { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Latest Notice Date")]
        public System.Nullable<DateTime> LatestNoticeDate { get; set; }
        [StringLength(64)]
        [Display(Name = "Latest Notice By")]
        public String LatestNoticeUser { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }

        //  **  Foreign Key **  //
        //[Key]
        //[ForeignKey("ConferenceID")]
        public int ConferenceID { get; set; }
        //  **  Entity Relationship **  //
        public virtual Conference Conference { get; set; }
    }

    public class Paper
    {
        //  **  Primary Key **  //
        [Display(Name = "System ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [StringLength(8)]
        [Display(Name = "Paper Number")]
        public String SequenceNumber { get; set; }
        [StringLength(256)]
        [Display(Name = "Title")]
        public String Title { get; set; }
        [StringLength(256)]
        [Display(Name = "Keywords")]
        public String Keywords { get; set; }
        [StringLength(64)]
        [Display(Name = "Fields / Technicals / Areas")]
        public String FieldCode { get; set; }           // Related Technical table
        [StringLength(64)]
        [Display(Name = "Style")]
        public String Style { get; set; }            //  Oral or Poster
        [StringLength(256)]
        [Display(Name = "Paper File")]
        public String FileNamePaper { get; set; }
        [StringLength(256)]
        [Display(Name = "Paper File Path")]
        public String FilePathPaper { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Upload Paper Date")]
        public System.Nullable<DateTime> DatePaper { get; set; }
        [StringLength(64)]
        [Display(Name = "Upload By")]
        public String UserPaper { get; set; }
        [StringLength(2000)]
        [Display(Name = "Remarks")]
        public String Remarks { get; set; }
        [StringLength(64)]
        [Display(Name = "TPC Status")]
        public String StatusTPC { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "TPC Date")]
        public System.Nullable<DateTime> DateTPC { get; set; }
        [StringLength(64)]
        [Display(Name = "TPC User")]
        public String UserTPC { get; set; }
        [StringLength(256)]
        [Display(Name = "Withdraw Reason (Option)")]
        public String ReasonWithdraw { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Withdraw Date")]
        public System.Nullable<DateTime> DateWithdraw { get; set; }
        [StringLength(64)]
        [Display(Name = "Withdraw User")]
        public String UserWithdraw { get; set; }
        [StringLength(256)]
        [Display(Name = "Copyright File")]
        public String FileNameCopyright { get; set; }
        [StringLength(256)]
        [Display(Name = "Copyright File Path")]
        public String FilePathCopyright { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Upload Copyright Date")]
        public System.Nullable<DateTime> DateCopyright { get; set; }
        [StringLength(64)]
        [Display(Name = "Upload Copyright By")]
        public String UserCopyright { get; set; }
        [StringLength(2000)]
        [Display(Name = "Abstract")]
        public String Abstract { get; set; }
        [StringLength(2000)]
        [Display(Name = "Biography")]
        public String Biography { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Confirm Date")]
        public System.Nullable<DateTime> DateConfirmed { get; set; }
        [StringLength(64)]
        [Display(Name = "Confirm User")]
        public String UserConfirmed { get; set; }

        //  **  Foreign Key **  //
        //[Key]
        //[ForeignKey("ConferenceID")]
        public int ConferenceID { get; set; }
        //  **  Entity Relationship **  //
        public virtual Conference Conference { get; set; }
        public virtual ICollection<Author> Authors { get; set; }
        public virtual ICollection<Observation> Observations { get; set; }
    }

    public class Author
    {
        //  **  Primary Key **  //
        [Display(Name = "System ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [StringLength(128)]
        [Display(Name = "User Account")]
        public string UserAccount { get; set; }
        [StringLength(64)]
        [Display(Name = "Authorization")]
        public string Authorization { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }

        //  **  Foreign Key **  //
        //[Key]
        //[ForeignKey("ConferenceID")]
        public int PaperID { get; set; }
        //  **  Entity Relationship **  //
        public virtual Paper Paper { get; set; }
    }
    public class Observation
    {
        //  **  Primary Key **  //
        [Display(Name = "System ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [StringLength(128)]
        [Display(Name = "User Account")]
        public string UserAccount { get; set; }
        [StringLength(64)]
        [Display(Name = "Participate")]
        public string Participate { get; set; }
        [Display(Name = "Grade")]
        public System.Nullable<int> Grade { get; set; }
        [StringLength(2000)]
        [Display(Name = "Comment")]
        public string Comment { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }

        //  **  Foreign Key **  //
        //[Key]
        //[ForeignKey("ConferenceID")]
        public int PaperID { get; set; }
        //  **  Entity Relationship **  //
        public virtual Paper Paper { get; set; }
    }

    public class Payment
    {
        //  **  Primary Key **  //
        [Display(Name = "System ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [StringLength(128)]
        [Display(Name = "User Account")]
        public string UserAccount { get; set; }
        [StringLength(64)]
        [Display(Name = "Member Number")]
        public string MemberNumber { get; set; }
        [StringLength(2000)]
        [Display(Name = "Affiliation")]
        public String Affiliation { get; set; }
        [StringLength(64)]
        [Display(Name = "Unit")]
        public String Unit { get; set; }        //  Current Unit (USD, HKD, ...)
        [Display(Name = "Amount")]
        public System.Nullable<Decimal> Amount { get; set; }
        [StringLength(128)]
        [Display(Name = "VPC Merch Reference")]
        public string MerchVPC { get; set; }
        [StringLength(256)]
        [Display(Name = "Payment File")]
        public string FileNamePayment { get; set; }
        [StringLength(256)]
        [Display(Name = "Payment File Path")]
        public string FilePathPayment { get; set; }
        [StringLength(64)]
        [Display(Name = "Check Results")]
        public String ResultPayment { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Check Payment Date")]
        public System.Nullable<DateTime> DatePayment { get; set; }
        [StringLength(64)]
        [Display(Name = "Check Payment By")]
        public String UserPayment { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }

        //  **  Foreign Key **  //
        //[Key]
        //[ForeignKey("ConferenceID")]
        public int ConferenceID { get; set; }
        //  **  Entity Relationship **  //
        public virtual Conference Conference { get; set; }
        public virtual ICollection<Payment> PaymentItems { get; set; }
        public virtual ICollection<PaymentVPC> PaymentVPCs { get; set; }
    }

    public class PaymentItem
    {
        //  **  Primary Key **  //
        [Display(Name = "System ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [StringLength(64)]
        [Display(Name = "Category")]
        public String Category { get; set; }
        [StringLength(64)]
        [Display(Name = "Unit")]
        public String Unit { get; set; }        //  Current Unit (USD, HKD, ...)
        [StringLength(64)]
        [Display(Name = "Type")]
        public String Type { get; set; }        //  Description of Account, Description of payment item
        [StringLength(128)]
        [Display(Name = "Description")]
        public String Description { get; set; }
        [Display(Name = "Price")]
        public System.Nullable<Decimal> Price { get; set; }
        [StringLength(64)]
        [Display(Name = "Enabled VPC")]
        public String EnabledVPC { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }

        //  **  Foreign Key **  //
        //[Key]
        //[ForeignKey("PaymentID")]
        public int PaymentID { get; set; }
        //  **  Entity Relationship **  //
        public virtual Payment Payment { get; set; }
    }

    public class PaymentVPC
    {
        //  **  Primary Key **  //
        [Display(Name = "System ID")]
        public int ID { get; set; }

        //  **  Attibute    ** //
        [StringLength(64)]
        [Display(Name = "Unit")]
        public String Unit { get; set; }        //  Current Unit (USD, HKD, ...)
        [StringLength(64)]
        [Display(Name = "Title")]
        public String Title { get; set; }
        [StringLength(256)]
        [Display(Name = "Name")]
        public String Name { get; set; }
        [StringLength(256)]
        [Display(Name = "Value")]
        public String Value { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public System.Nullable<DateTime> DateCreate { get; set; }
        [StringLength(64)]
        [Display(Name = "Create User")]
        public String UserCreate { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Edit Date")]
        public System.Nullable<DateTime> DateEdit { get; set; }
        [StringLength(64)]
        [Display(Name = "Edit User")]
        public String UserEdit { get; set; }

        //  **  Foreign Key **  //
        //[Key]
        //[ForeignKey("ConferenceID")]
        public int ConferenceID { get; set; }
        //  **  Entity Relationship **  //
        public virtual Conference Conference { get; set; }
    }

    public class SelectedListItem
    {
        public int ID { get; set; }
        public int SequenceNumber { get; set; }
        public string ItemText { get; set; }
        public string ItemValue { get; set; }
        public decimal ItemDecimal { get; set; }
        public bool ItemChecked { get; set; }
    }

    public class ViewModel_ConferenceProfileSearch
    {
        public UserInformation UserInformation { get; set; }
        public List<ViewModel_ConferenceProfile> ViewModel_ConferenceProfiles { get; set; }
    }

    public class ViewModel_ConferenceProfile
    {
        public Conference Confernece { get; set; }
        public List<ControlDate> ControlDates { get; set; }
        public int NumCallOfPaper { get; set; }
        public int NumOfAcceptedPaper { get; set; }
        public int NumOfRegistered { get; set; }
    }

    public class ViewModel_ConferencePriceItems
    {
        [StringLength(64)]
        [Display(Name = "Unit")]
        public String Unit { get; set; }        //  Current Unit (USD, HKD, ...)
        public ControlDate ControlDate { get; set; }
        public List<PriceItem> PriceItems { get; set; }
    }

    public class ViewModel_ConferenceVPC
    {
        public Conference Confernece { get; set; }
        [StringLength(64)]
        [Display(Name = "Unit")]
        public String Unit { get; set; }        //  Current Unit (USD, HKD, ...)
        public List<ConfigureVPC> ConfigureVPCs { get; set; }

    }

    public class ViewModel_Conference
    {
        public Conference Confernece { get; set; }
        public List<ControlDate> ControlDates { get; set; }
        public List<Field> Fields { get; set; }
        public List<SelectedListItem> SelectStyles { get; set; }
        public List<ViewModel_ConferencePriceItems> ViewModel_ConferencePriceItems { get; set; }
        public int NumberOfSubmittedPaper { get; set; }
        public int NumberOfAcceptedPaper { get; set; }
        public int NumberOfSubmittedPayment { get; set; }
        public int NumberOfAcceptedPayment { get; set; }

    }

    public class ViewModel_PaperProfile
    {
        public Conference ConferenceProfile { get; set; }
        public List<ViewModel_Paper> ViewModel_Paper { get; set; }

        public List<SelectedListItem> SelectTPCStatus { get; set; }
    }


    public class ViewModel_Paper
    {
        public Conference Conference { get; set; }
        public Paper Paper { get; set; }
        [Display(Name = "Nature of payment")]
        public int NumPaid { get; set; }
        public List<Author> EditAuthors { get; set; }
        public List<Observation> EditObservations { get; set; }
        public List<ViewModel_Author> ViewModel_Author { get; set; }
        public List<ViewModel_Observation> ViewModel_Observation { get; set; }

        public List<SelectedListItem> SelectStyles { get; set; }
        [Display(Name = "Permission Group")]
        public String PermissionGroup { get; set; }
        public List<SelectedListItem> SelectTPCStatus { get; set; }
    }

    public class ViewModel_Author
    {
        public Author Author { get; set;    }
        public UserInformation UserInformation { get; set; }
    }

    public class ViewModel_Observation
    {
        public Observation Observation { get; set; }
        public Committee Committee { get; set; }
        public UserInformation UserInformation { get; set; }
    }

    public class ViewModel_ReviewerAssignment
    {
        public Conference Conference { get; set; }
        public Paper Paper { get; set; }
        public String PermissionGroup {get; set;}
        public List<ViewModel_Observation> ViewModel_Observation { get; set; }
        public List<SelectedListItem> SelectGroups { get; set; }
    }

}

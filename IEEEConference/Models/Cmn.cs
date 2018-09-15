using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;


namespace IEEEConference.Models
{
    class Cmn
    {
        private DateTime currentDatetime { get; set; }
        private string userName { get; set; }
        public Cmn()
        {
            currentDatetime = DateTime.Now;
            return;
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        public DateTime CurrentDatetime
        {
            get { return currentDatetime; }
        }

        public int CnvInteger(String input)
        {
            if (input == null) { return 0; }
            int numVal = -1;
            try
            {
                numVal = Convert.ToInt32(input);
                return numVal;
            }
            catch (FormatException e)
            {
                return 0;
            }
        }

        public decimal CnvDecimal(String input)
        {
            if (input == null) { return 0; }

            decimal numVal = -1;
            try
            {
                numVal = decimal.Parse(input);
                return numVal;
            }
            catch (FormatException e)
            {
                return 0;
            }
        }


        public string CnvString(String input, int numDigit)
        {
            int inputInt = 0;
            if (numDigit == null || numDigit == 0)
            {
                numDigit = 2;
            }
            if (IsInteger(input) == true)
            {
                inputInt = CnvInteger(input);
            }
            string versionNum = "00000000" + inputInt.ToString();
            return versionNum.Substring(versionNum.Length - numDigit);
        }

        public Nullable<DateTime> CnvDateFormat(DateTime input)
        {
            if (input == null) { return null; }
            string vDateString = input.ToString("dd MMM yyyy");
            vDateString = vDateString.ToUpper();
            DateTime dt = Convert.ToDateTime(vDateString);
            return dt;
        }

        public DateTime CnvDateTime(String dtInput, String dtLimit)
        {
            if (dtInput == null) { dtInput = dtLimit; };
            string dtString = dtInput.ToUpper();
            DateTime dt = Convert.ToDateTime(dtString);
            return dt;
        }

        public String GetItemErrorMessage(String item, String quantity, String unit, String description, String price)
        {
            //  blank record
            if ((item == null || item.Length == 0) && (quantity == null || quantity.Length == 0) && (unit == null || unit.Length == 0) && (description == null || description.Length == 0) && (price == null || price.Length == 0))
            { return null; }
            //  Description only
            if ((item == null || item.Length == 0) && (quantity == null || quantity.Length == 0) && (unit == null || unit.Length == 0) && (description.Length > 0) && (price == null || price.Length == 0))
            { return ""; }
            //  Item record
            if (item.Length > 0 && quantity.Length > 0 && unit.Length > 0 && description.Length > 0 && price.Length > 0)
            { return ""; }
            //  Invalid record
            return "*";
        }

        public String GetWorkOrderReimbursementItemErrorMessage(String item, String unit, String description, String amount)
        {
            //  blank record
            if ((item == null || item.Length == 0) && (unit == null || unit.Length == 0) && (description == null || description.Length == 0) && (amount == null || amount.Length == 0))
            { return null; }
            //  Description only
            if ((item == null || item.Length == 0) && (unit == null || unit.Length == 0) && (description.Length > 0) && (amount == null || amount.Length == 0))
            { return ""; }
            //  Item record
            if (item.Length > 0 && unit.Length > 0 && description.Length > 0 && amount.Length > 0)
            { return ""; }
            //  Invalid record
            return "*";
        }

        public String GetWorkOrderItemErrorMessage(String item, String orderIssueTo, String instructions, String description, String targetDate, String estimateManHour)
        {
            //  blank record
            if ((item == null || item.Length == 0) && (orderIssueTo == null || orderIssueTo.Length == 0) && (instructions == null || instructions.Length == 0) && (description == null || description.Length == 0) && (targetDate == null || targetDate.Length == 0) && (estimateManHour == null || estimateManHour.Length == 0))
            { return null; }
            //  Description only
            if ((item == null || item.Length == 0) && (orderIssueTo == null || orderIssueTo.Length == 0) && (instructions == null || instructions.Length == 0) && (description.Length > 0) && (targetDate == null || targetDate.Length == 0) && (estimateManHour == null || estimateManHour.Length == 0))
            { return ""; }
            //  Item record
            if (item.Length > 0 && orderIssueTo.Length > 0 && instructions.Length > 0 && description.Length > 0 && targetDate.Length > 0 && estimateManHour.Length > 0)
            { return ""; }
            //  Invalid record
            return "*";
        }

        public String GetMaterialRequisitionItemErrorMessage(String item, String price, String qty, String description, String unit)
        {
            //  blank record
            if ((item == null || item.Length == 0) && (price == null || price.Length == 0) && (qty == null || qty.Length == 0) && (description == null || description.Length == 0) && (unit == null || unit.Length == 0))
            { return null; }
            //  Description only
            if ((item == null || item.Length == 0) && (price == null || price.Length == 0) && (qty == null || qty.Length == 0) && (description.Length != null) && (unit == null || unit.Length == 0))
            { return ""; }
            //  Description only
            if ((item == null || item.Length == 0) || (price == null || price.Length == 0) || (qty == null || qty.Length == 0) || (unit == null || unit.Length == 0))
            { return "*"; }
            //  Item record
            if (item.Length > 0 && price.Length > 0 && qty.Length > 0 && description.Length > 0 && unit.Length > 0 )
            { return ""; }
            //  Invalid record
            return "*";
        }

        public String GetMaterialReviewItemErrorMessage(String item, String type, String qty, String description, String unit)
        {
            //  blank record
            if ((item == null || item.Length == 0) && (type == null || type.Length == 0) && (qty == null || qty.Length == 0) && (description == null || description.Length == 0) && (unit == null || unit.Length == 0))
            { return null; }
            //  Description only
            if ((item == null || item.Length == 0) && (type == null || type.Length == 0) && (qty == null || qty.Length == 0) && (description.Length > 0) && (unit == null || unit.Length == 0))
            { return ""; }
            //  Item record
            if (item.Length > 0 && type.Length > 0 && qty.Length > 0 && description.Length > 0 && unit.Length > 0)
            { return ""; }
            //  Invalid record
            return "*";
        }

        public string GetFinanceYear()
        {
            string tempStr = "";
            int tempInt = 0;
            if (currentDatetime.Month >= 7)
            {
                tempStr = currentDatetime.Year.ToString();
                return tempStr.Substring(tempStr.Length - 2);
            }
            tempInt = currentDatetime.Year - 1;
            tempStr = tempInt.ToString();
            return tempStr.Substring(tempStr.Length - 2);
        }

        public String GetVersionID(String numRecord, String maxVersion, int numDigit)
        {
            int recordInt = 0;
            if (numDigit == null || numDigit == 0)
            {
                numDigit = 2;
            }
            if (IsInteger(numRecord) == true)
            {
                recordInt = CnvInteger(numRecord);
            }
            int versionInt = 0;
            if (IsInteger(maxVersion) == true)
            {
                versionInt = CnvInteger(maxVersion);
            }
            if (versionInt < recordInt)
            {
                versionInt = recordInt;
            }
            versionInt = versionInt + 1;
            string versionNum = "00000000" + versionInt.ToString();
            return versionNum.Substring(versionNum.Length - numDigit);
        }

        public string GetReportFileName(string projectFolderID, string quotationCode, string projectCode)
        {
            if (projectFolderID == null) { return ""; };
            string projectFolderIDStr = "00000000" + projectFolderID.ToString();

            string quotationCodeStr;
            if ((quotationCode == null) || (quotationCode == ""))
            { quotationCodeStr = ""; }  else { quotationCodeStr = "_" + quotationCode; }

            string projectCodeStr;
            if ((projectCode == null) || (projectCode == "") )
            { projectCodeStr = ""; } else { projectCodeStr = "_" + projectCode;  }

            string fileName = projectFolderIDStr.Substring(projectFolderIDStr.Length - 5) + quotationCodeStr + projectCodeStr;
            return fileName;
        }

        public string GetFileName(string className, string docType, string conferenceCode, string numVersion, string specifiedCode, string docNameExtension)
        {
            string fileName;
            string specifiedCodeStr;
            if (conferenceCode == null) { return ""; };
            string projectFolderIDStr = "00000000" + conferenceCode.ToString();
            if (docNameExtension == null || docNameExtension == "") { return ""; }

            specifiedCodeStr = specifiedCode;
            if (specifiedCodeStr == null) { specifiedCodeStr = ""; };

            fileName = projectFolderIDStr.Substring(projectFolderIDStr.Length - 6) + "_" + className + "_" + numVersion + "_" + docType + "_" + specifiedCodeStr + "_" + GetRandomHash() + docNameExtension;
            return fileName;
        }

        public string GetStoredFilePath()
        {
            return "~/DocFile/Stored";
        }

        public string GetRandomHash()
        {
            String CurrentDT = DateTime.Now.ToString("0:yyyy MMMM dd HH:mm:ss");
            String Md5Hash = GetMd5Hash(CurrentDT);
            return Md5Hash;
        }
        public string GetMd5Hash(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString().ToUpper();
        }

        public Boolean IsInteger(String input)
        {
            int result;
            Boolean inputStatus;
            inputStatus = Int32.TryParse(input, out result);
            return inputStatus;
        }

        // Verify a hash against a string. 
        public bool VerifyMd5Hash(string input, string hash)
        {
            MD5 md5Hash = MD5.Create();

            // Hash the input. 
            string hashOfInput = GetMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            return false;
        }


    }
}
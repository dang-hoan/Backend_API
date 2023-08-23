using Domain.Constants;
using Domain.Entities.Employee;
using Domain.Wrappers;

namespace Domain.Helpers
{
    public class StringHelper
    {
        private static readonly string[] VietNamChar = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };

        public static string ConvertFromVietnameseText(string str)
        {
            //Thay thế và lọc dấu từng char
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            return str;
        }

        // Ex: keyword is 'Luận' then result is 'Luận', 'Luận123', ...
        //     keyword is 'Luan' then result is 'Luận', 'Luan', 'Luận123', ...
        public static bool Contains(string data, string keyword)
        {
            data = data.ToLower(); keyword = keyword.ToLower();            
            string keywordTMP = ConvertFromVietnameseText(keyword);

            if (!keywordTMP.Equals(keyword)) // => keyword is accented string
            {
                if (data.Contains(keyword))
                    return true;
            }
            else
            {
                if (ConvertFromVietnameseText(data).Contains(keyword))
                    return true;
            }

            return false;
        }
        public static string CheckLimitEmployee(Domain.Entities.Employee.Employee employee)
        {
            if (employee.Name.Length > 100) return StaticVariable.LIMIT_NAME;
            if (employee.Email.Length > 100) return StaticVariable.LIMIT_EMAIL;
            if (employee.Address != null)
            {
                if (employee.Address.Length > 500) return StaticVariable.LIMIT_ADDRESS;
            }
            if (employee.Image != null)
            {
                if (employee.Image.Length > 200) return StaticVariable.LIMIT_IMAGE;
            }
            return "";
        }
        public static string CheckLimitWorkShift(Domain.Entities.WorkShift.WorkShift workShift)
        {
            if (workShift.Name.Length > 100) return StaticVariable.LIMIT_NAME;
            if (workShift.Description != null)
            {
                if (workShift.Description.Length > 500) return StaticVariable.LIMIT_DESCRIPTION;
            }
            return "";
        }
        public static string CheckLimitService(Domain.Entities.Service.Service service)
        {
            if (service.Name.Length > 100) return StaticVariable.LIMIT_NAME;
            if(service.Description != null)
            {
                if (service.Description.Length > 500) return StaticVariable.LIMIT_DESCRIPTION;
            }
            return "";
        }
        public static string CheckLimitCustomer(Domain.Entities.Customer.Customer customer)
        {
            if (customer.CustomerName.Length > 100) return StaticVariable.LIMIT_NAME;
            if (customer.Address != null)
            {
                if (customer.Address.Length > 500) return StaticVariable.LIMIT_ADDRESS;
            }
            return "";
        }
    }
}
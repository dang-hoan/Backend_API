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
    }
}
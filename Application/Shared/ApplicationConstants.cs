namespace Application.Shared
{
    public static class ApplicationConstants
    {
        public static class SuccessMessage
        {
            public const string CreatedSuccess = "Dữ liệu được tạo thành công.";
            public const string UpdatedSuccess = "Dữ liệu được cập nhật thành công.";
            public const string DeletedSuccess = "Dữ liệu được xoá thành công.";
        }

        public static class ErrorMessage
        {
            public const string NotFound = "Dữ liệu không được tìm thấy.";
            public const string SystemError = "Lỗi hệ thống.";
            public const string InvalidFile = "Tệp không hợp lệ.";
            public const string InCorrectFile = "Tệp tải lên không đúng.";
            public const string NotDelete = "Dữ liệu đã từng được sử dụng nên không thể xoá.";
            public const string DevicesNotDelete = "Thiết bị đã từng được sử dụng nên không thể xoá.";

        }
    }
}
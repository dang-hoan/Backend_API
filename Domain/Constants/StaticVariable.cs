namespace Domain.Constants
{
    public class StaticVariable
    {
        // System
        public const string SHORT_NAME_APP = "NPLUS";

        // Permission
        public const string ACCESS = "Access";
        public const string ADD = "Add";
        public const string EDIT = "Edit";
        public const string DELETE = "Delete";

        // url of page to Authorization
        public const string EMPLOYEE_LIST = "/employee-management";

        //reset password
        public const string RESET_PASSWORD = "Abc123!@#";

        //error message

        public const string NOT_FOUND_MSG = "Not found match data!";
        public const string SERVER_ERROR_MSG = "Internal server error!";
        public const string INVALID_PASSWORD = "Password must be at least 8 characters.";
        public const string IS_EXISTED_USERNAME = "Username already exists in the system.";
        public const string ERROR_ADD_USER = "There was an error during the account creation process.";

        public const string EMAIL_EXISTS_MSG = "Email already exists in the database.";

        public const string USERNAME_EXISTS_MSG = "Username already exists in the database.";

        public const string PHONE_ERROR_MSG = "The number of digits in a phone number must be between 8 and 10.";
        public const string NOT_FOUND_SERVICE = "This service does not exist in the database.";
        public const string NOT_FOUND_CUSTOMER = "This customer does not exist in the database.";
        public const string NOT_FOUND_BOOKING = "This booking does not exist in the database.";
        public const string NOT_FOUND_WORK_SHIFT = "This work shift does not exist in the database.";
        public const string NOT_FOUND_BOOKING_DETAIL = "This booking detail does not exist in the database.";
        public const string SUCCESS = "Success";

        public const string WORK_SHIFT_ASSIGNED = "This work shift has assigned to employees! Please change employee's work shift before delete this work shift!";
        public const string NOT_LOGIC_WORKING_TIME = "Working time must not be empty if work shift isn't default!!";
        public const string NOT_LOGIC_WORKING_TIME_FORMAT = "Working time format must be 'HH:mm'!";
        public const string NOT_LOGIC_DATE_ORDER = "ToTime must be greater than FromTime";
        public const string NOT_LOGIC_WORKDAY_VALUE = "Workdays must be between 2 and 8!";

        public const string IS_NOT_LOGIN = "You must login to do this action";

        public const string NOT_HAVE_ACCESS = "You don't have permission to access";

    }
}
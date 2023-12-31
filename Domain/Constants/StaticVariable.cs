﻿namespace Domain.Constants
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
        public const string REQUIRED_USER_NAME = "User name is required";
        public const string REQUIRED_PASSWORD = "Password is required";

        public const string INVALID_PHONE_NUMBER = "Phone number is invalid";
        public const string INVALID_EMAIL = "Email is invalid";
        public const string INVALID_USER_NAME = "User name is invalid";
        public const string INVALID_PASSWORD = "Password is invalid";

        public const string NOT_FOUND_MSG = "Not found match data!";
        public const string SERVER_ERROR_MSG = "Internal server error!";
        public const string INVALID_PASSWORD_LONG = "Password must be at least 8 characters.";
        public const string IS_EXISTED_USERNAME = "Username already exists in the system.";
        public const string ERROR_ADD_USER = "There was an error during the account creation process.";
        public const string PHONE_NUMBER_EXISTS_MSG = "Phone number already exists in the database.";

        public const string EMAIL_EXISTS_MSG = "Email already exists in the database.";

        public const string USERNAME_EXISTS_MSG = "Username already exists in the database.";

        public const string PHONE_ERROR_MSG = "The number of digits in a phone number must be between 8 and 10.";
        public const string NOT_FOUND_SERVICE = "This service does not exist in the database.";
        public const string NOT_FOUND_CUSTOMER = "This customer does not exist in the database.";
        public const string NOT_FOUND_BOOKING = "This booking does not exist in the database.";
        public const string NOT_FOUND_WORK_SHIFT = "This work shift does not exist in the database.";
        public const string NOT_FOUND_BOOKING_DETAIL = "This booking detail does not exist in the database.";
        public const string NOT_FOUND_FEEDBACK = "This feedback does not exist in the database.";
        public const string SUCCESS = "Success";

        public const string WORK_SHIFT_ASSIGNED = "This work shift has assigned to employees! Please change employee's work shift before delete this work shift!";
        public const string NOT_LOGIC_WORKING_TIME = "Working time must not be empty if work shift isn't default!!";
        public const string NOT_LOGIC_WORKING_TIME_FORMAT = "Working time format must be 'HH:mm'!";
        public const string NOT_SAME_DAY = "FromTime and ToTime field must be on the same day!";
        public const string NOT_LOGIC_DATE_ORDER = "ToTime must be greater than FromTime";
        public const string NOT_LOGIC_BOOKING_DATE = "Booking date must be less than service time";
        public const string DATETIME_LESS_THAN_CURRENT = "Selected datetime must be greater than current datetime!";
        public const string NOT_LOGIC_WORKDAY_VALUE = "Workdays must be between 2 and 8!";

        public const string IS_NOT_LOGIN = "You must login to do this action";

        public const string NOT_HAVE_ACCESS = "You don't have permission to access";

        public const string STATUS_NOT_EXIST = "Status is not exist";

        public const string ENUM_MUST_NOT_BE_EDITTED = "You must not edit this enum because it is basic enum!";
        
        public const string ENUM_MUST_NOT_BE_DELETED = "You must not delete this enum because it is basic enum!";

        //Enum
        public const string WAITING = "waiting";
        public const string INPROGRESSING = "inprogressing";
        public const string DONE = "done";

        public const string RATING_ENUM = "Rating enum";
        public const string BOOKING_STATUS_ENUM = "Booking status enum";

        //Limit character
        public const string LIMIT_NAME = "The name should not exceed 100 characters.";
        public const string LIMIT_ADDRESS = "The address should not exceed 500 characters.";
        public const string LIMIT_EMAIL = "The email should not exceed 100 characters.";
        public const string LIMIT_USERNAME = "The username should not exceed 50 characters.";
        public const string LIMIT_PASSWORD = "The password should not exceed 100 characters.";
        public const string LIMIT_IMAGE = "The image should not exceed 200 characters.";
        public const string LIMIT_DESCRIPTION = "The description should not exceed 500 characters.";
        public const string LIMIT_NOTE = "The note should not exceed 500 characters.";
    }
}
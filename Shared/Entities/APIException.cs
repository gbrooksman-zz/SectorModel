using System;


namespace SectorModel.Shared.Entities
{
    public class APIException : Exception
    {
       // private int _number;
      //  private string _description;
        public APIException()
        {
            
        }

        public APIException(string message) : base(message)
        {
           this.APIExceptionDescription = message;
        }

        public APIException(string message, Exception inner) : base(message, inner)
        {
            this.APIExceptionDescription = message;
        }

        public APIException(int apiNumber, string apiDescription, string message, Exception inner) : base(message, inner)
        {
           this.APIExceptionNumber = apiNumber;
           this.APIExceptionDescription = apiDescription;
        }


        public APIException(int apiNumber, string apiDescription)
        {
           this.APIExceptionNumber = apiNumber;
           this.APIExceptionDescription = apiDescription;
        }

        public int APIExceptionNumber { get; set; }

        public string APIExceptionDescription { get; set; }

        //===================================================================================================
        // this section contains custom error numbers and related messages
        //===================================================================================================
        
        
        public const int EQUITY_USED = 1000;
        public const string EQUITY_USED_MESSAGE = "The equity is used in one or more models";

    }

}
using System;

namespace CLICKA.TechnicalAssessment.Model
{
    public class PingData
    {
        public String Endpoint { get; set; }
        public Boolean IsSuccess { get; set; }
        public string Duration { get; set; }
        public string ErrorMessage { get; set; }
    }
}

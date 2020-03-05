using System;

namespace SCTimeSheet_DAL.Models
{
    public class EntityBase
    {
        public Int64? CreatedBy { get; set; }
        
        public DateTime? CreatedDate { get; set; }
        
        public Int64? ModifiedBy { get; set; }
        
        public DateTime? ModifiedDate { get; set; }

        public class NotTracked : Attribute {  }
    }
}

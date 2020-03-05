using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCTimeSheet_DAL.Models
{
    public class OnBehalfRadGridModel
    {
        public IEnumerable<NewEntryByProjectSelection> Products { get; set; }
        public IEnumerable<NewEntryByProjectSelection> SelectedProducts { get; set; }
    }
}

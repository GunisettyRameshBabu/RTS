using SCTimeSheet_DAL;
using SCTimeSheet_DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SCTimeSheet_HELPER
{
    public class AutoGen
    {
        public static string GetReferenceNumber()
        {
            ApplicationDBContext DB = new ApplicationDBContext();
            string refNumber = string.Empty;
            int seqNumber = 0;
            string date = DateTime.Now.ToString("ddMMyyyy");

            AutoGenModel refObj = new AutoGenModel();
            refObj = DB.AutoGen.FirstOrDefault();
            if (refObj != null)
            {
                if (refObj.Date.Date == DateTime.Today)
                {
                    seqNumber = refObj.RefID + 1;
                    refObj.RefID = seqNumber;
                }
                else
                {
                    seqNumber = 1;
                    refObj.Date = DateTime.Now;
                    refObj.RefID = seqNumber;
                }

                DB.AutoGen.Attach(refObj);
                DB.Entry(refObj).State = EntityState.Modified;
                DB.SaveChanges();
            }
            string totlSeqNumbr = "";
            int digitcount = seqNumber.ToString().Length;
            if (digitcount == 1)
            {
                totlSeqNumbr = "000" + seqNumber;
            }
            else if (digitcount == 2)
            {
                totlSeqNumbr = "00" + seqNumber;
            }
            else if (digitcount == 3)
            {
                totlSeqNumbr = "0" + seqNumber;
            }
            switch (digitcount)
            {
                case 1:
                    totlSeqNumbr = "000" + seqNumber;
                    break;

                case 2:
                    totlSeqNumbr = "00" + seqNumber;
                    break;

                case 3:
                    totlSeqNumbr = "0" + seqNumber;
                    break;

                default:
                    totlSeqNumbr = Convert.ToString(seqNumber);
                    break;
            }

            refNumber = "TS" +date+ totlSeqNumbr;

            return refNumber;
        }
    }
}

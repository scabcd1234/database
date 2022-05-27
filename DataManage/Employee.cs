using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DataManage
{
    public class Employee
    {
        private string strTitle;
        private DateTime objBirthday;
        public string Title

        {

            get { return strTitle; }

            set

            {

                strTitle = value;

                if (String.IsNullOrEmpty(strTitle))

                {

                    throw new ApplicationException("Please input Title.");

                }

            }

        }



        public DateTime Birthday

        {

            get { return objBirthday; }

            set

            {

                objBirthday = value;

                if (objBirthday.Year >= DateTime.Now.Year)

                {

                    throw new ApplicationException("Please enter a valid date.");

                }

            }

        }
    }
}

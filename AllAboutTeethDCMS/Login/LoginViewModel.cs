using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Login
{
    public class LoginViewModel : PageViewModel
    {
        private AllAboutTeeth alAboutTeeth;

        public AllAboutTeeth AlAboutTeeth { get => alAboutTeeth; set => alAboutTeeth = value; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebForm.Data.Interfaces;

namespace WebForm.App
{
    public partial class _Default : Page
    {
        [Import] public IRepository _repository { get; set; }        

        protected void Page_Load(object sender, EventArgs e)
        {
            this._repository.Add();
        }
    }
}
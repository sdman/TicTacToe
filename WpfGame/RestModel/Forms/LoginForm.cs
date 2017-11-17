using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WpfGame.RestModel.Forms
{
    class LoginForm
    {
        public string Name { get; set; }

        public LoginForm(string name)
        {
            Name = name;
        }
    }
}
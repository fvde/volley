using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;

namespace Torrero.Model
{
    [DataContract]
    public class HighscoreItem
    {
        #region Properties

        [DataMember]
        public String Name
        {
            get { return this.name; }
            set{this.name= value;}
        }

        [DataMember]
        public int Score
        {
            get { return this.score; }
            set { this.score = value; }
        }

        [DataMember]
        public int Place
        {
            get { return this.place; }
            set { this.place = value; }
        }
        #endregion

        #region Members

        private int score;
        private String name;
        private int place;

        #endregion

        #region Initialization

        
        public HighscoreItem(int place, String name, int score)
        {
            this.name = name;
            this.score = score;
            this.place = place;
        }
        #endregion
    }
}

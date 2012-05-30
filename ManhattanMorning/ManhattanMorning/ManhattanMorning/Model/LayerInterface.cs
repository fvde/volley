using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManhattanMorning.Misc;

namespace ManhattanMorning.Model
{
    /// <summary>
    /// Interface that is used from every Object we want to add to our LayerList.
    /// </summary>
    public interface LayerInterface
    {
        /// <summary>
        /// Layer of the Object.
        /// Values 0-100. See Layer doc for more Information.
        /// </summary>
        int Layer { get; set; }

        /// <summary>
        /// Name of the Object.
        /// </summary>
        String Name { get;}

    }
}

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

namespace QuizProject.Tools
{
    public static class RandomExtensions
    {
        /// <summary>
        /// shuffels a given array
        /// Usage:
        /// var array = new int[] {1, 2, 3, 4};
        /// new Random().Shuffle(array);
        /// </summary>
        /// <typeparam name="T">type of the array to be shuffled</typeparam>
        /// <param name="rng">a random generator</param>
        /// <param name="array">the array to be shuffled</param>
        public static void shuffleArray<T>(this Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}

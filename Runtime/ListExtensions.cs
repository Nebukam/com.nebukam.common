using System.Collections.Generic;

namespace Nebukam
{
    public static class ListsExtensions
    {


        /// <summary>
        /// Return the last item of the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T Last<T>(this IList<T> @this)
        {
            int index = @this.Count - 1;
            if (index < 0) { return default(T); }

            return @this[index];
        }

        /// <summary>
        /// Return the first item of the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T First<T>(this IList<T> @this)
        {
            if (@this.Count == 0) { return default(T); }
            return @this[0];
        }

        /// <summary>
        /// Return an item at random index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T RandomPick<T>(this IList<T> @this)
        {
            return Nebukam.Collections.Lists.RandomPick(@this);
        }

        /// <summary>
        /// Return and remove and item at a random index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T RandomExtract<T>(this IList<T> @this)
        {
            return Nebukam.Collections.Lists.RandomExtract(@this);
        }

        /// <summary>
        /// Randomize this list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        public static void Randomize<T>(this IList<T> @this)
        {
            Nebukam.Collections.Lists.Randomize(@this);
        }

        /// <summary>
        /// Removes and return the last item of the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T Pop<T>(this IList<T> @this)
        {
            int index = @this.Count - 1;
            if (index < 0) { return default(T); }

            T result = @this[index];
            @this.RemoveAt(index);
            return result;
        }

        /// <summary>
        /// Removes and return the first item of the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T Shift<T>(this IList<T> @this)
        {

            if (@this.Count == 0) { return default(T); }

            T result = @this[0];
            @this.RemoveAt(0);
            return result;
        }

        /// <summary>
        /// Only add an item if it isn't already in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T AddOnce<T>(this IList<T> @this, T item)
        {
            if (@this.Contains(item)) { return item; }
            @this.Add(item);
            return item;
        }

        /// <summary>
        /// Only add an item if it isn't already in the list, with a boolean
        /// returning whether or not the iteam was already in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="item"></param>
        /// <param name="alreadyPresent"></param>
        /// <returns></returns>
        public static T AddOnce<T>(this IList<T> @this, T item, out bool alreadyPresent)
        {
            if (@this.Contains(item))
            {
                alreadyPresent = true;
                return item;
            }

            alreadyPresent = false;
            @this.Add(item);
            return item;
        }

        /// <summary>
        /// Returns whether the list is empty or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IList<T> @this)
        {
            return !(@this.Count != 0);
        }

    }
}

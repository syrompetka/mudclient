// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MatchingResult.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the MatchingResult type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils.PatternMatching
{
    using CSLib.Net.Annotations;

    /// <summary>
    /// A result of string matching.
    /// </summary>
    public struct MatchingResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether match was was successfull.
        /// </summary>
        /// <value>
        /// <c>true</c> if match wass successfull; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the start position.
        /// </summary>
        /// <value>
        /// The start position.
        /// </value>
        public int StartPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the end position.
        /// </summary>
        /// <value>
        /// The end position.
        /// </value>
        public int EndPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(MatchingResult leftValue, MatchingResult rightValue)
        {
            return leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="leftValue">The left value to compare.</param>
        /// <param name="rightValue">The right value to compare.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(MatchingResult leftValue, MatchingResult rightValue)
        {
            return !leftValue.Equals(rightValue);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != typeof(MatchingResult))
            {
                return false;
            }

            return Equals((MatchingResult)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="MatchingResult"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="MatchingResult"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="MatchingResult"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(MatchingResult other)
        {
            return other.IsSuccess.Equals(IsSuccess) && other.StartPosition == StartPosition && other.EndPosition == EndPosition;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = IsSuccess.GetHashCode();
                result = (result * 397) ^ StartPosition;
                result = (result * 397) ^ EndPosition;
                return result;
            }
        }
    }
}

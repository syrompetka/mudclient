using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Adan.Client.Common.Model
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUndo
    {
        /// <summary>
        /// 
        /// </summary>
        UndoOperation Operation { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Group Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string UndoInfo();

        /// <summary>
        /// 
        /// </summary>
        void Undo();     
    }

    /// <summary>
    /// 
    /// </summary>
    public enum UndoOperation
    {
        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        Add,

        /// <summary>
        /// 
        /// </summary>
        Remove,
    }
}

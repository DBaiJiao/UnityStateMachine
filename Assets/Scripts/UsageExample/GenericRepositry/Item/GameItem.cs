using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtilities.GenericRepository.Items
{
    public class GameItem : BaseRepositoryItem
    {
        private ItemType itemType;

        public ItemType ItemType => itemType;



        public GameItem() : base() {}

        public GameItem(string displayName) : base(displayName) {}

        public GameItem(string id, string displayName) : base(id, displayName) {}
        

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <returns>克隆的对象</returns>
        public override IRepositoryItem Clone()
        {
            return new GameItem(DisplayName);
        }
    }
}

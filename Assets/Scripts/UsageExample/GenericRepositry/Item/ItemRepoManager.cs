using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UnityUtilities.GenericRepository.Items
{
    public class ItemRepoManager : MonoBehaviour
    {
        public static ItemRepoManager Instance;

        private ItemRepositry itemRepositry;


        public ItemRepositry ItemRepositry => itemRepositry;
        public List<string> GameItemNames = new List<string>();


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            InitItemRepositry();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                AddGameItem();
            }
        }

        void InitItemRepositry()
        {
            itemRepositry = new ItemRepositry();

            ItemRepositry.OnItemAdded += UpdateGameItemNames;
            ItemRepositry.OnItemUpdated += UpdateGameItemNames;
            ItemRepositry.OnItemRemoved += UpdateGameItemNames;
        }

        void UpdateGameItemNames(GameItem gameItem)
        {
            GameItemNames = itemRepositry.GetAll().Select(item => item.DisplayName).ToList();
        }

        void AddGameItem()
        {
            GameItem gameItem = new GameItem("Test Item");
            itemRepositry.Add(gameItem);
        }


    }
}
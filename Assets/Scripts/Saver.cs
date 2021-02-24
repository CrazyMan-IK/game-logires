using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CrazyGames.Logires.Models;
using CrazyGames.Logires.Interfaces;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

namespace CrazyGames.Logires
{
    public class Saver
    {
        private static Saver _instance = null;

        public static Saver Instance
        {
            get
            {
                if (_instance == null) _instance = new Saver();
                return _instance;
            }
        }

        private readonly string _savesDirectory = null;

        public int LastIndex
        {
            get
            {
                var files = Directory.GetFiles(_savesDirectory, "*.json").Select(x => Path.GetFileName(x)).Where(x => Regex.IsMatch(x, @"save[0-9]+\.json")).Select(x => Convert.ToInt32(x.Replace("save", "").Replace(".json", "")));

                if (files.Count() > 0)
                {
                    return files.Max(x => x);
                }

                return -1;
            }
        }
        public Metadata CurrentLevelMetadata { get; private set; } = new Metadata();

        private Saver() 
        {
            _savesDirectory = Path.Combine(Application.persistentDataPath, "Saves");
        }

        public IEnumerator LoadScene(int index)
        {
            UnityEngine.Object.Destroy(Camera.main.gameObject);

            var loading = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            var scene = SceneManager.GetSceneAt(1);
            //loading.allowSceneActivation = false;

            while (!loading.isDone)
            {
                yield return null;
            }

            var root = scene.GetRootGameObjects().Select(x => x.GetComponent<BlocksRoot>()).First(x => x != null);
            LoadToRoot(root.transform, index);

            SceneManager.UnloadSceneAsync(0);

            //loading.allowSceneActivation = true;
        }

        public void LoadToRoot(Transform root, int index)
        {
            Save save;

            try
            {
                save = Load(index);
            }
            catch
            {
                save = null;
            }

            if (save != null)
            {
                var blocks = new List<Block>();

                foreach (var block in save.Blocks)
                {
                    var prefab = Inventory.Instance.GetBlock(block.BlockID);
                    if (prefab != null)
                    {
                        var _block = UnityEngine.Object.Instantiate(prefab, block.Position, prefab.transform.rotation, root);
                        BlocksRoot.Instance.AddBlock(_block);
                        _block.name = _block.name.Replace("(Clone)", "");

                        blocks.Add(_block);
                    }
                    else
                    {
                        blocks.Add(null);
                    }
                }

                for (var i = 0; i < save.Blocks.Count; i++)
                {
                    var sblock = save.Blocks[i];
                    var block = blocks[i];

                    if (block != null)
                    {
                        for (var j = 0; j < sblock.Inputs.Count; j++)
                        {
                            var input = sblock.Inputs[j];

                            if (blocks.Count > input.LinkedBlockIndex &&
                                block is IHaveInputs haveInputs &&
                                blocks[input.LinkedBlockIndex] is IHaveOutputs haveOutputs)
                            {
                                if (haveInputs.Inputs.Count > j && haveOutputs.Outputs.Count > input.LinkedPinIndex)
                                {
                                    haveInputs.Inputs[j].ConnectTo(haveOutputs.Outputs[input.LinkedPinIndex]);
                                }
                            }
                        }
                    }
                }

                CurrentLevelMetadata.Index = index;
                CurrentLevelMetadata.Title = save.Title;
            }
            else
            {
                CurrentLevelMetadata.Index = LastIndex + 1;
                CurrentLevelMetadata.Title = "<Unknown>";
            }
        }

        public void SaveCurrentLevel()
        {
            var save = new Save();

            for (int i = 0; i < BlocksRoot.Instance.Blocks.Count; i++)
            {
                var blockSave = new Save.BlockSave();

                var block = BlocksRoot.Instance.Blocks[i];
                var trans = block.transform;

                blockSave.BlockID = block.GetID();
                blockSave.Position = trans.position;

                if (block is IHaveInputs haveInputs)
                {
                    foreach (var input in haveInputs.Inputs)
                    {
                        if (input.LinkedPins.Count > 0)
                        {
                            var linkedBlock = input.LinkedPins[0].GetComponentInParent<Block>();
                            var linkedBlockIndex = BlocksRoot.Instance.Blocks.ToList().IndexOf(linkedBlock);
                            var linkedPinIndex = ((IHaveOutputs)linkedBlock).Outputs.ToList().IndexOf(input.LinkedPins[0]);
                            blockSave.Inputs.Add(new Save.BlockSave.LinkedPin() { LinkedBlockIndex = linkedBlockIndex, LinkedPinIndex = linkedPinIndex });
                        }
                    }
                }

                if (block is IHaveOutputs haveOutputs)
                {
                    foreach (var output in haveOutputs.Outputs)
                    {
                        if (output.LinkedPins.Count > 0)
                        {
                            var links = new List<Save.BlockSave.LinkedPin>();
                            foreach (var linkedPin in output.LinkedPins)
                            {
                                var linkedBlock = linkedPin.GetComponentInParent<Block>();
                                var linkedBlockIndex = BlocksRoot.Instance.Blocks.ToList().IndexOf(linkedBlock);
                                var linkedPinIndex = ((IHaveInputs)linkedBlock).Inputs.ToList().IndexOf(linkedPin);
                                links.Add(new Save.BlockSave.LinkedPin() { LinkedBlockIndex = linkedBlockIndex, LinkedPinIndex = linkedPinIndex });
                            }
                            blockSave.Outputs.Add(links);
                        }
                    }
                }

                save.Blocks.Add(blockSave);
            }

            if (CurrentLevelMetadata.Index == -1)
            {
                CurrentLevelMetadata.Index = LastIndex + 1;
                save.Title = "<Unknown>";
            }
            else
            {
                save.Title = CurrentLevelMetadata.Title;
            }

            Save(save, CurrentLevelMetadata.Index);
        }

        public IEnumerator<int> GetAllSaves()
        {
            var files = Directory.GetFiles(_savesDirectory, "*.json").Select(x => Path.GetFileName(x)).Where(x => Regex.IsMatch(x, @"save[0-9]+\.json"));

            foreach (var file in files)
            {
                yield return Convert.ToInt32(file.Replace("save", "").Replace(".json", ""));
            }
        }

        public void Save(Save save, int index = -1)
        {
            var data = JsonConvert.SerializeObject(save, Formatting.Indented);

            string filename;
            if (index >= 0)
            {
                filename = $"save{index}.json";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            Directory.CreateDirectory(_savesDirectory);

            File.WriteAllText(Path.Combine(_savesDirectory, filename), data);
        }

        public void DeleteSave(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            string filePath = Path.Combine(_savesDirectory, $"save{index}.json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public Save Load(int index)
        {
            Directory.CreateDirectory(_savesDirectory);

            var data = File.ReadAllText(Path.Combine(_savesDirectory, $"save{index}.json"));

            return JsonConvert.DeserializeObject<Save>(data);
        }
    }
}

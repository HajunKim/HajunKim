﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace MText
{
    [CreateAssetMenu(fileName = "New 3D Font", menuName = "Modular 3d Text/New Font")]
    public class MText_Font : ScriptableObject
    {
        public List<MText_Character> characters = new List<MText_Character>();
        [SerializeField] GameObject fontSet = null;
        [Tooltip("When set to false, new fontset will be added while keeping the old one")]
        [SerializeField] bool overwriteOldSet = true;

        [Tooltip("A font where each character is spaced equally. If turned on, individual spacing value from list below is ignored")]
        public bool monoSpaceFont;
        public bool useUpperCaseLettersIfLowerCaseIsMissing = true;
        [Tooltip("Word spacing and spacing for unavailable characters")]
        public float emptySpaceSpacing = 1;

        [Tooltip("Text's character spacing = font's character spacing * text's character spacing")]
        public float characterSpacing = 1;

        public Vector3 rotationFix;
        public Vector3 positionFix;
        public Vector3 scaleFix;

        public bool enableKerning = true;
        public float kerningMultiplier = 1f;
        //unfortunately dictionary isn't serializable //TODO
        public List<MText_KernPairHolder> kernTable = new List<MText_KernPairHolder>();


        public Mesh RetrievePrefab(char c)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                if (c == characters[i].character)
                {
                    return MeshPrefab(i);
                }
            }

            if (useUpperCaseLettersIfLowerCaseIsMissing)
            {
                if (char.IsLetter(c))
                {
                    c = char.ToUpper(c);
                }
                for (int i = 0; i < characters.Count; i++)
                {
                    if (c == characters[i].character)
                    {
                        return MeshPrefab(i);
                    }
                }
            }

            return null;
        }

        Mesh MeshPrefab(int i)
        {
            if (characters[i].prefab)
            {
                if (characters[i].prefab.GetComponent<MeshFilter>())
                {
                    if (characters[i].prefab.GetComponent<MeshFilter>().sharedMesh)
                    {
                        return characters[i].prefab.GetComponent<MeshFilter>().sharedMesh;
                    }
                }
            }
            else if (characters[i].meshPrefab)
            {
                return characters[i].meshPrefab;
            }


            return null;
        }

        public float Spacing(char c)
        {
            if (!monoSpaceFont)
            {
                for (int i = 0; i < characters.Count; i++)
                {
                    if (c == characters[i].character)
                    {
                        return characters[i].spacing * characterSpacing;
                    }
                }
            }
            return emptySpaceSpacing * characterSpacing;
        }
        public float Spacing(char previousChar, char currentChar)
        {
            if (!enableKerning || kernTable.Count == 0)
            {
                return Spacing(currentChar);
            }

            if (!monoSpaceFont)
            {
                for (int i = 0; i < characters.Count; i++)
                {
                    if (currentChar == characters[i].character)
                    {
                        float kerning = Kerning(previousChar, currentChar);
                        Debug.Log(kerning);
                        return characters[i].spacing * characterSpacing * kerning;
                    }
                }
            }
            return emptySpaceSpacing * characterSpacing;
        }

        float Kerning(char previousChar, char currentChar)
        {
            MText_KernPair kernPair = new MText_KernPair();
            kernPair.left = Character(previousChar).glyphIndex;
            kernPair.right = Character(currentChar).glyphIndex;

            for (int i = 0; i < kernTable.Count; i++)
            {
                if (kernTable[i].kernPair.left == kernPair.left && kernTable[i].kernPair.right == kernPair.right)
                {
                    return 1 + (kernTable[i].offset * kerningMultiplier * 0.01f);
                }
            }

            return 1;
        }

        MText_Character Character(char c)
        {
            for (int i = 0; i < characters.Count; i++)
            {
                if(characters[i].character == c)
                {
                    return characters[i];
                }
            }
            return null;
        }

        public void UpdateCharacterList(GameObject prefab)
        {
            fontSet = prefab;
            UpdateCharacterList();
        }

        public void UpdateCharacterList()
        {
            if (overwriteOldSet)
                characters.Clear();

            if (fontSet)
            {
                foreach (Transform child in fontSet.transform)
                {
                    AddCharacter(child.gameObject);
                }
            }
            else
            {
                Debug.LogWarning("Fontset not found on " + name);
            }

            AverageSpacing();
        }

        public void AddCharacter(GameObject obj)
        {
            MText_Character newChar = new MText_Character();
            if (!obj)
                return;

            char character;
            float spacing;

            ProcessName(obj.name, out character, out spacing);

            newChar.character = character;
            newChar.spacing = spacing;
            newChar.prefab = obj;

            characters.Add(newChar);
        }
        public void AddCharacter(Mesh mesh)
        {
            MText_Character newChar = new MText_Character();

            if (!mesh)
                return;

            char character;
            float spacing;

            ProcessName(mesh.name, out character, out spacing);

            newChar.character = character;
            newChar.spacing = spacing;


            newChar.meshPrefab = mesh;

            characters.Add(newChar);
        }



        private void ProcessName(string name, out char character, out float spacing)
        {
            if (name.Contains("dot"))
            {
                character = '.';
                spacing = (float)Convert.ToDouble(name.Substring(4));

            }
            else if (name.Contains("forwardSlash"))
            {
                character = '/';
                spacing = (float)Convert.ToDouble(name.Substring(13));
            }
            else if (name.Contains("quotationMark"))
            {
                character = '"';
                spacing = (float)Convert.ToDouble(name.Substring(14));
            }
            else if (name.Contains("multiply"))
            {
                character = '*';
                spacing = (float)Convert.ToDouble(name.Substring(9));
            }
            else if (name.Contains("colon"))
            {
                character = ':';
                spacing = (float)Convert.ToDouble(name.Substring(6));
            }
            else if (name.Contains("lessThan"))
            {
                character = '<';
                spacing = (float)Convert.ToDouble(name.Substring(9));
            }
            else if (name.Contains("moreThan"))
            {
                character = '>';
                spacing = (float)Convert.ToDouble(name.Substring(9));
            }
            else if (name.Contains("questionMark"))
            {
                character = '?';
                spacing = (float)Convert.ToDouble(name.Substring(13));
            }
            else if (name.Contains("slash"))
            {
                character = '/';
                spacing = (float)Convert.ToDouble(name.Substring(6));
            }
            else if (name.Contains("backwardSlash"))
            {
                character = '\\';
                spacing = (float)Convert.ToDouble(name.Substring(14));
            }
            else if (name.Contains("verticalLine"))
            {
                character = '|';
                spacing = (float)Convert.ToDouble(name.Substring(13));
            }
            else
            {
                char[] chars = name.ToCharArray();
                character = chars[0];
                spacing = (float)Convert.ToDouble(name.Substring(2));
            }
            spacing *= 0.81f;
        }

        [ContextMenu("Get average spacing")]
        private void AverageSpacing()
        {
            float total = 0;
            for(int i = 0; i < characters.Count; i++)
            {
                total += characters[i].spacing;
            }
            emptySpaceSpacing = total / characters.Count;
        }
    }
}
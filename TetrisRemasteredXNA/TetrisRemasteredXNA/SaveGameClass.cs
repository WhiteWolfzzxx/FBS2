﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TetrisRemasteredXNA
{
    public class SaveGameClass
    {
        FileStream theFileRead;
        FileStream theFileWrite;
        StreamReader saveRead;
        StreamWriter saveWrite;
        private bool[,] so = new bool[10, 20];

        public bool getSo() { return so[9, 19]; }

        public SaveGameClass()
        {

        }

        public void Update(GameTime gameTime)
        {

        }

        public void recordGameData(bool[,] st)
        {
            try
            {
                theFileWrite = new FileStream("savedGameData.txt",
                                              FileMode.Create,
                                              FileAccess.Write);
                saveWrite = new StreamWriter(theFileWrite);

                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 20; y++)
                    {
                        saveWrite.WriteLine(st[x, y].ToString());
                    }
                }

                saveWrite.Close();
                theFileWrite.Close();
            }
            catch
            {

            }
        }

        public bool[,] loadGameData()
        {
            try
            {
                theFileRead = new FileStream("savedGameData.txt",
                                             FileMode.Open,
                                             FileAccess.Read);
                saveRead = new StreamReader(theFileRead);

                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 20; y++)
                    {
                        so[x, y] = Convert.ToBoolean(saveRead.ReadLine());
                    }
                }

                saveRead.Close();
                theFileRead.Close();
            }
            catch
            {

            }

            return so;
        }
    }
}



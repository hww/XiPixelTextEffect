
using UnityEngine;

namespace XiPixelTextEffect
{
    /// <summary>
    /// Similar to old fasion pixels screen where we can draw the text
    /// </summary>
    [System.Serializable]
    public class PixelTextBuilder
    {
        public const int DEFAULT_CHAR_W_PIXELS = 8;
        public const int DEFAULT_CHAR_H_PIXELS = 8;
        
        // Actual tiles count
        private int tilesCount;
        
        // Full storeage for the tilePositions
        public Vector3[] tilePositions;    
        
        // Actual usage the tilePositions table
        private Bounds tilesBounds = new Bounds();

        // Constructor 
        public PixelTextBuilder(int capacity = 256) 
        {
            SetCapacity(capacity); 
        }
        
        // Get actual tiles count
        public int TilesCount => tilesCount;

        // Get the bounding volume of the tiles
        public Bounds Bounds => tilesBounds;

        // Set the maximul storage for the tiles (save to resize)
        public void SetCapacity(int capacity)
        {
            if (tilePositions == null)
            {
                tilePositions = new Vector3[capacity];
                return;
            }
            if (capacity > tilePositions.Length)
            {
                var old = tilePositions;
                tilePositions = new Vector3[capacity];
                System.Array.Copy(old, tilePositions, old.Length);
            }
        }

        // Clear all tiles
        public void Clear()
        {
            tilesCount = 0;
            tilesBounds.SetMinMax(Vector3.zero, Vector3.zero);
        }

        // Print the text with centering it
        // TODO Make multiline support!
        public void PrintStringCentered(string text, Vector3 worldPos, Vector2 tileSize)
        {
            Debug.Assert(text != null);
            Debug.Assert(!text.Contains('\n'));

            var textWidth = DEFAULT_CHAR_W_PIXELS * tileSize.x * text.Length;
            var textHeiht = DEFAULT_CHAR_H_PIXELS * tileSize.y /* *1 line */;
            var newPos = new Vector3(worldPos.x - textWidth * 0.5f,
                                     worldPos.y + textHeiht * 0.5f,
                                     worldPos.z);
            PrintString(text, newPos, tileSize);
        }

        private void PrintString(string text, Vector3 worldPos, Vector2 tileSize)
        {
            Debug.Assert(text != null);

            var wPos = worldPos;
            foreach (var c in text)
            {
                switch (c)
                {
                    case ' ':
                        wPos.x += DEFAULT_CHAR_W_PIXELS * tileSize.x;
                        break;
                    case '\n':
                        wPos.x = worldPos.x;
                        wPos.y += DEFAULT_CHAR_H_PIXELS * tileSize.y;
                        break;
                    default:
                        PrintChar(c, wPos, tileSize);
                        wPos.x += DEFAULT_CHAR_W_PIXELS * tileSize.x;
                        break;
                }
            }
        }

        private void PrintChar(char c, Vector3 worldPos, Vector3 worldSize)
        {
            byte[] font = null;
            var idx = BitmapFontHeader.GetCharacter(c, ref font);
            var wy = worldPos.y;
            var oldTileNum  = tilesCount;
            for (var y=0; y<DEFAULT_CHAR_H_PIXELS; y++)
            {
                var x = 0;
                var pixels = (int)font[idx++];
                while (pixels > 0)
                {
                    if ((pixels & 1) > 0)
                    {
                        tilePositions[tilesCount] = new Vector3(worldPos.x + x * worldSize.x, worldPos.y - y * worldSize.y);
                        tilesCount++;
                        if (tilesCount >= tilePositions.Length)
                            SetCapacity(tilePositions.Length + 64);
                    }
                    x++;
                    pixels = pixels >> 1;
                }
            }
            for (var i = oldTileNum; i < tilesCount; i++)
                tilesBounds.Encapsulate(tilePositions[i]);
        }

    }
}

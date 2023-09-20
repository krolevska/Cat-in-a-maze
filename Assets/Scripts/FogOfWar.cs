using UnityEngine;
namespace CatInMaze
{
    public class FogOfWar : MonoBehaviour
    {
        public Transform player; // Reference to player
        public int revealRadius = 3; // Radius around the player to reveal

        private Texture2D fogTexture;
        private Color[] clearCircle;

        void Start()
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            // Create a copy of the fog texture
            fogTexture = new Texture2D((int)renderer.sprite.rect.width, (int)renderer.sprite.rect.height);
            Color[] originalPixels = renderer.sprite.texture.GetPixels();
            fogTexture.SetPixels(originalPixels);
            fogTexture.Apply();

            renderer.sprite = Sprite.Create(fogTexture, renderer.sprite.rect, new Vector2(0.5f, 0.5f));
            Bounds fogBounds = GetComponent<SpriteRenderer>().bounds;
            Vector2 normalizedPosition = new Vector2(
    (player.position.x - fogBounds.min.x) / fogBounds.size.x,
    (player.position.y - fogBounds.min.y) / fogBounds.size.y);
            int textureX = Mathf.FloorToInt(normalizedPosition.x * fogTexture.width);
            int textureY = Mathf.FloorToInt(normalizedPosition.y * fogTexture.height);

            clearCircle = CreateGradientClearCircle(revealRadius);
        }

        void Update()
        {
            ClearFogAroundPlayer();
        }

        private Color[] CreateGradientClearCircle(int radius)
        {
            Color[] circle = new Color[2 * radius * 2 * radius];

            for (int y = -radius; y < radius; y++)
            {
                for (int x = -radius; x < radius; x++)
                {
                    float distanceFromCenter = Mathf.Sqrt(x * x + y * y);
                    float alpha = Mathf.Clamp01(1.0f - (distanceFromCenter / radius));

                    if (distanceFromCenter <= radius)
                        circle[(x + radius) + (y + radius) * 2 * radius] = new Color(0, 0, 0, alpha);
                    else
                        circle[(x + radius) + (y + radius) * 2 * radius] = Color.black;
                }
            }

            return circle;
        }


        private void ClearFogAroundPlayer()
        {
            Bounds fogBounds = GetComponent<SpriteRenderer>().bounds;

            Vector2 normalizedPosition = new Vector2(
                (player.position.x - fogBounds.min.x) / fogBounds.size.x,
                (player.position.y - fogBounds.min.y) / fogBounds.size.y);

            int textureX = Mathf.FloorToInt(normalizedPosition.x * fogTexture.width);
            int textureY = Mathf.FloorToInt(normalizedPosition.y * fogTexture.height);

            int startX = Mathf.Clamp(textureX - revealRadius, 0, fogTexture.width - 2 * revealRadius);
            int startY = Mathf.Clamp(textureY - revealRadius, 0, fogTexture.height - 2 * revealRadius);

            Color[] currentPixels = fogTexture.GetPixels(startX, startY, 2 * revealRadius, 2 * revealRadius);

            for (int i = 0; i < currentPixels.Length; i++)
            {
                currentPixels[i].a = Mathf.Min(currentPixels[i].a, clearCircle[i].a);
            }

            fogTexture.SetPixels(startX, startY, 2 * revealRadius, 2 * revealRadius, currentPixels);
            fogTexture.Apply();
        }



    }
}
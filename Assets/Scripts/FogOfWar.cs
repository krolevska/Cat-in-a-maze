using UnityEngine;

namespace CatInMaze
{
    public class FogOfWar : MonoBehaviour
    {
        public Transform player; // Reference to the player's position
        public int revealRadius = 3; // Radius around the player to reveal

        private Texture2D fogTexture; // Texture used for fog of war
        private Color[] clearCircle; // Array representing the clear circle around the player

        void Start()
        {
            // Get the SpriteRenderer component attached to this GameObject
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            // Create a copy of the fog texture to work with
            fogTexture = new Texture2D((int)renderer.sprite.rect.width, (int)renderer.sprite.rect.height);
            Color[] originalPixels = renderer.sprite.texture.GetPixels();
            fogTexture.SetPixels(originalPixels);
            fogTexture.Apply();

            // Apply the modified texture to the SpriteRenderer
            renderer.sprite = Sprite.Create(fogTexture, renderer.sprite.rect, new Vector2(0.5f, 0.5f));

            // Calculate the position of the player relative to the fog bounds
            Bounds fogBounds = GetComponent<SpriteRenderer>().bounds;
            Vector2 normalizedPosition = new Vector2(
                (player.position.x - fogBounds.min.x) / fogBounds.size.x,
                (player.position.y - fogBounds.min.y) / fogBounds.size.y);

            // Calculate the initial clear circle for the fog
            clearCircle = CreateGradientClearCircle(revealRadius);
        }

        void Update()
        {
            // Continuously update the fog of war based on the player's position
            ClearFogAroundPlayer();
        }

        private Color[] CreateGradientClearCircle(int radius)
        {
            // Create a gradient clear circle based on distance from the center
            Color[] circle = new Color[2 * radius * 2 * radius];

            for (int y = -radius; y < radius; y++)
            {
                for (int x = -radius; x < radius; x++)
                {
                    float distanceFromCenter = Mathf.Sqrt(x * x + y * y);
                    float alpha = Mathf.Clamp01(0.0f - (distanceFromCenter / radius));

                    if (distanceFromCenter <= radius)
                        circle[(x + radius) + (y + radius) * 2 * radius] = new Color(0, 0, 0, alpha); // Set the pixel color with gradient alpha
                    else
                        circle[(x + radius) + (y + radius) * 2 * radius] = Color.black; // Set pixels outside the circle to fully opaque black
                }
            }

            return circle;
        }

        private void ClearFogAroundPlayer()
        {
            // Calculate the position of the player relative to the fog bounds
            Bounds fogBounds = GetComponent<SpriteRenderer>().bounds;
            Vector2 normalizedPosition = new Vector2(
                (player.position.x - fogBounds.min.x) / fogBounds.size.x,
                (player.position.y - fogBounds.min.y) / fogBounds.size.y);

            // Calculate the region to clear based on the player's position
            int textureX = Mathf.FloorToInt(normalizedPosition.x * fogTexture.width);
            int textureY = Mathf.FloorToInt(normalizedPosition.y * fogTexture.height);
            int startX = Mathf.Clamp(textureX - revealRadius, 0, fogTexture.width - 2 * revealRadius);
            int startY = Mathf.Clamp(textureY - revealRadius, 0, fogTexture.height - 2 * revealRadius);

            // Get the current pixels within the clearing region
            Color[] currentPixels = fogTexture.GetPixels(startX, startY, 2 * revealRadius, 2 * revealRadius);

            // Update the alpha value of the pixels based on the clear circle
            for (int i = 0; i < currentPixels.Length; i++)
            {
                currentPixels[i].a = Mathf.Min(currentPixels[i].a, clearCircle[i].a);
            }

            // Apply the updated pixels back to the fog texture
            fogTexture.SetPixels(startX, startY, 2 * revealRadius, 2 * revealRadius, currentPixels);
            fogTexture.Apply();
        }
    }
}

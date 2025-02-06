using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceWar
{
    public static class Helper
    {
        /// <summary>
        /// Returns a random float value between the vector's x and y values.
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        public static float RandomValue(this Vector2 _value)
        {
            return Random.Range(_value.x, _value.y);
        }

        public static float Lerp(this Vector2 _value, float _tValue)
        {
            return Mathf.Lerp(_value.x, _value.y, _tValue);
        }

        public static float Lerp(this Vector2Int _value, float _tValue)
        {
            return Mathf.Lerp(_value.x, _value.y, _tValue);
        }

        /// <summary>
        /// Returns a random integer value between the vector's x and y values.
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        public static int RandomValue(this Vector2Int _value)
        {
            return Random.Range(_value.x, _value.y + 1);
        }

        public static Vector2 RandomVector2(this Vector2 _value)
        {
            return new Vector2(Random.Range(-_value.x, _value.x), Random.Range(-_value.y, _value.y));
        }

        /// <summary>
        /// Reverses the provided.
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        public static BulletUser ReverseUser(BulletUser _user)
        {
            return _user == BulletUser.Player ? BulletUser.Enemy : BulletUser.Player;
        }

        /// <summary>
        /// Shuffles the list randomly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            // basically swapping between two element in each iteration
            for (int i = 0; i < list.Count; i++)
            {
                int k = Random.Range(0, list.Count);
                T value = list[k];
                list[k] = list[i];
                list[i] = value;
            }
        }

        /// <summary>
        /// Returns a random item in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RandomItem<T>(this IList<T> list)
        {
            // basically swapping between two element in each iteration
            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Returns a random item in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="removeItem">If set to true, the selected item is removed from the list.</param>
        /// <returns></returns>
        public static T RandomItem<T>(this IList<T> list, bool removeItem)
        {
            T selectedItem = list[Random.Range(0, list.Count)];

            if (removeItem)
            {
                list.Remove(selectedItem);
            }

            return selectedItem;
        }

        public static List<T> Duplicate<T>(this IList<T> list)
        {
            List<T> newArray = new List<T>();

            foreach (T item in list)
            {
                newArray.Add(item);
            }

            return newArray;
        }

        public static int GetAverageArea(this PolygonCollider2D _col)
        {
            return (int)((_col.bounds.size.x + _col.bounds.size.y) / 2);
        }

        /// <summary>
        /// Colors the string value based on the color selected.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="_color">The new color of the string.</param>
        /// <returns></returns>
        public static string Color(this string text, Color _color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(_color)}>{text}</color>";
        }

        public static List<T> ArrayToList<T>(T[] array)
        {
            List<T> list = new List<T>();

            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }

            return list;
        }

        public static float GetTrueSpeed(float _speed, float _distance)
        {
            return (LevelDimensions.Instance.LevelWidth / _speed) * (_distance / LevelDimensions.Instance.LevelWidth);
        }

        public static string GetTimeValue(float _seconds)
        {
            int hours;
            int minutes;
            int seconds;
            hours = (int)(_seconds / 3600f);
            _seconds -= hours * 3600f;
            minutes = (int)(_seconds / 60f);
            _seconds -= minutes * 60f;
            seconds = (int)_seconds;
            return $"{hours:00}: {minutes:00}: {seconds:00}";
        }

        public static float GetNextValueWithinRange(float _value, float _addition, float _maxValue, float _minValue)
        {
            _value += _addition;

            if (_value > _maxValue) _value = _minValue;

            if (_value < _minValue) _value = _maxValue;

            return _value;
        }


        #region Ship Methods
        public static float GetPolygonColliderSize(PolygonCollider2D collider)
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            for (int i = 0; i < collider.pathCount; i++)
            {
                Vector2[] points = collider.GetPath(i);

                foreach (Vector2 point in points)
                {
                    min = Vector2.Min(min, point);
                    max = Vector2.Max(max, point);
                }
            }

            Vector2 size = new Vector2(max.x - min.x, max.y - min.y);
            float sizeNumber = Mathf.Max(size.x, size.y);
            Debug.Log($"{sizeNumber}. X = {size.x}, Y = {size.y}".Color(UnityEngine.Color.green));
            return sizeNumber;
        }

        public static Vector2 GetRandomPositionOnSprite(SpriteRenderer _spriteRenderer, SpriteRandomPointPosition _style)
        {
            Texture2D texture;
            float x = _spriteRenderer.transform.position.x;
            float y = _spriteRenderer.transform.position.y;

            bool IsPointWithinSprite(Vector2 point)
            {
                Vector2 localPoint = point - (Vector2)_spriteRenderer.transform.position;
                Vector2 spriteSize = _spriteRenderer.bounds.size;
                Vector2 spritePoint = new Vector2(localPoint.x / spriteSize.x + 0.5f, localPoint.y / spriteSize.y + 0.5f);
                spritePoint.x *= texture.width;
                spritePoint.y *= texture.height;

                if (!texture.isReadable)
                {
                    Debug.Log($"Text is not readable: {texture.name}");
                }

                Color pixelColor = texture.GetPixel((int)spritePoint.x, (int)spritePoint.y);
                return pixelColor.a > 0;
            }

            Vector2 getRandomPosition()
            {
                switch (_style)
                {
                    case SpriteRandomPointPosition.RandonXAndY:
                        {
                            x = Random.Range(_spriteRenderer.bounds.min.x, _spriteRenderer.bounds.max.x);
                            y = Random.Range(_spriteRenderer.bounds.min.y, _spriteRenderer.bounds.max.y);
                            break;
                        }
                    case SpriteRandomPointPosition.RandomXStableY:
                        {
                            x = Random.Range(_spriteRenderer.bounds.min.x, _spriteRenderer.bounds.max.x);
                            break;
                        }
                    case SpriteRandomPointPosition.StableXRandomY:
                        {
                            y = Random.Range(_spriteRenderer.bounds.min.y, _spriteRenderer.bounds.max.y);
                            break;
                        }
                    case SpriteRandomPointPosition.OuterXOuterY:
                        {
                            x = Random.Range(x, _spriteRenderer.bounds.max.x);
                            y = Random.Range(y, _spriteRenderer.bounds.max.y);
                            break;
                        }

                    case SpriteRandomPointPosition.OuterXRandomY:
                        {
                            x = Random.Range(x, _spriteRenderer.bounds.max.x);
                            y = Random.Range(_spriteRenderer.bounds.min.y, _spriteRenderer.bounds.max.y);
                            break;
                        }
                    case SpriteRandomPointPosition.RandomXOuterY:
                        {
                            x = Random.Range(_spriteRenderer.bounds.min.x, _spriteRenderer.bounds.max.x);
                            y = Random.Range(y + (_spriteRenderer.bounds.max.y - y) * 0.8f, _spriteRenderer.bounds.max.y);
                            break;
                        }
                    case SpriteRandomPointPosition.StableXOuterY:
                        {
                            y = Random.Range(y + (_spriteRenderer.bounds.max.y - y) * 0.8f, _spriteRenderer.bounds.max.y);
                            break;
                        }
                    case SpriteRandomPointPosition.OuterXStableY:
                        {
                            x = Random.Range(x, _spriteRenderer.bounds.max.x);
                            break;
                        }
                }

                return new Vector2(x, y);
            }

            texture = _spriteRenderer.sprite.texture;
            Vector2 randomPoint;
            int counter = 0;

            do
            {
                counter++;
                randomPoint = getRandomPosition();

                if (counter > 1000)
                {
                    Debug.LogError($"The sprite: ({_spriteRenderer.gameObject.name}) with the texture: {texture.name} doesn't have a spot on the sprite.");
                    Debug.Break();
                    break;
                }
            }
            while (!IsPointWithinSprite(randomPoint));

            return randomPoint;
        }

        public static GameObject CreateCombinedSprites(SpriteRenderer[] _spriteRenderers)
        {
            #region Old
            //// Calculate the total size of the combined texture
            //int combinedWidth = 0;
            //int combinedHeight = 0;
            //foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
            //{
            //    if (spriteRenderer.sprite != null)
            //    {
            //        combinedWidth += spriteRenderer.sprite.texture.width;
            //        combinedHeight = Mathf.Max(combinedHeight, spriteRenderer.sprite.texture.height);
            //    }
            //}

            //// Create a new Texture2D to store the combined sprites
            //Texture2D combinedTexture = new Texture2D(combinedWidth, combinedHeight);

            //// Calculate the x offset for each sprite in the combined texture
            //int xOffset = 0;

            //// Loop through all the sprite renderers and add their sprites to the combined texture
            //foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
            //{
            //    if (spriteRenderer.sprite != null)
            //    {
            //        // Get the sprite texture
            //        Texture2D spriteTexture = spriteRenderer.sprite.texture;

            //        // Get the sprite pixels
            //        Color[] spritePixels = spriteTexture.GetPixels((int)spriteRenderer.sprite.textureRect.x,
            //                                                       (int)spriteRenderer.sprite.textureRect.y,
            //                                                       (int)spriteRenderer.sprite.textureRect.width,
            //                                                       (int)spriteRenderer.sprite.textureRect.height);

            //        // Set the sprite pixels in the combined texture
            //        combinedTexture.SetPixels(xOffset, 0, spriteTexture.width, spriteTexture.height, spritePixels);

            //        // Update the x offset for the next sprite
            //        xOffset += spriteTexture.width;
            //    }
            //}

            //// Apply the changes to the combined texture
            //combinedTexture.Apply();

            //// Create a new sprite to use the combined texture as its texture
            //Sprite combinedSprite = Sprite.Create(combinedTexture, new Rect(0, 0, combinedWidth, combinedHeight), new Vector2(0.5f, 0.5f), 100);

            //// Create a new GameObject with a SpriteRenderer component to display the combined sprite
            //GameObject combinedSpriteObject = new GameObject("Combined Sprite");
            //SpriteRenderer combinedSpriteRenderer = combinedSpriteObject.AddComponent<SpriteRenderer>();
            //combinedSpriteRenderer.sprite = combinedSprite;

            //// Move the GameObject to the center of the screen for display
            //combinedSpriteObject.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            #endregion
            // Get all the SpriteRenderer components in the scene

            // Calculate the total size of the combined texture
            int combinedWidth = 0;
            int combinedHeight = 0;
            foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
            {
                if (spriteRenderer.sprite != null)
                {
                    combinedWidth += spriteRenderer.sprite.texture.width;
                    combinedHeight = Mathf.Max(combinedHeight, spriteRenderer.sprite.texture.height);
                }
            }

            // Create a new Texture2D to store the combined sprites
            Texture2D combinedTexture = new Texture2D(combinedWidth, combinedHeight);

            // Create a list to store the positions of the individual sprites
            List<Vector3> spritePositions = new List<Vector3>();

            // Calculate the x offset for each sprite in the combined texture
            int xOffset = 0;

            // Loop through all the sprite renderers and add their sprites to the combined texture
            foreach (SpriteRenderer spriteRenderer in _spriteRenderers)
            {
                if (spriteRenderer.sprite != null)
                {
                    // Get the sprite texture
                    Texture2D spriteTexture = spriteRenderer.sprite.texture;

                    // Get the sprite pixels
                    Color[] spritePixels = spriteTexture.GetPixels((int)spriteRenderer.sprite.textureRect.x,
                                                                   (int)spriteRenderer.sprite.textureRect.y,
                                                                   (int)spriteRenderer.sprite.textureRect.width,
                                                                   (int)spriteRenderer.sprite.textureRect.height);

                    // Set the sprite pixels in the combined texture
                    combinedTexture.SetPixels(xOffset, 0, spriteTexture.width, spriteTexture.height, spritePixels);

                    // Add the position of the sprite to the list
                    spritePositions.Add(spriteRenderer.transform.position);

                    // Update the x offset for the next sprite
                    xOffset += spriteTexture.width;
                }
            }

            // Apply the changes to the combined texture
            combinedTexture.Apply();

            // Create a new sprite to use the combined texture as its texture
            Sprite combinedSprite = Sprite.Create(combinedTexture, new Rect(0, 0, combinedWidth, combinedHeight), new Vector2(0.5f, 0.5f), 100);

            // Create a new GameObject with a SpriteRenderer component to display the combined sprite
            GameObject combinedSpriteObject = new GameObject("Combined Sprite");
            SpriteRenderer combinedSpriteRenderer = combinedSpriteObject.AddComponent<SpriteRenderer>();
            combinedSpriteRenderer.sprite = combinedSprite;

            // Loop through the positions of the individual sprites and set the position of the combined sprite
            for (int i = 0; i < spritePositions.Count; i++)
            {
                Vector3 position = spritePositions[i];
                position.x -= combinedWidth / 2;
                position.y -= combinedHeight / 2;
                position.z = 0;
                position.x += i * combinedTexture.width / spritePositions.Count;
                combinedSpriteObject.transform.position = position;
            }

            return combinedSpriteObject;
        }

        public static void SetPolygonCollider(PolygonCollider2D _collider, List<ShipBuilder.PolygonInfo> _scaledColliders)
        {
            PolygonCollider2D collider = _collider;
            // we set the number of paths the ship will have which is the number of the polygon colliders cached during the ship creation iterations
            collider.pathCount = _scaledColliders.Count;

            for (int i = 0; i < _scaledColliders.Count; i++)
            {
                // we use the scale path function before setting the path to the main ship because the wings and bodies usually have their scale change which doesn't change the scale of the collider
                collider.SetPath(i, ScalePath(_scaledColliders[i].Polygon.GetPath(0), _scaledColliders[i].Scale, _scaledColliders[i].Offset));
            }
        }

        public static void SetPolygonCollider(PolygonCollider2D _collider, List<PolygonCollider2D> _colliders)
        {
            List<ShipBuilder.PolygonInfo> colInfo = new List<ShipBuilder.PolygonInfo>();

            for (int i = 0; i < _colliders.Count; i++)
            {
                // collect polygon colliders from all ship parts and remove it from the parts (to be assembled later in the main body)
                if (_colliders[i] != null)
                {
                    colInfo.Add(new ShipBuilder.PolygonInfo
                    {
                        Polygon = _colliders[i],
                        Scale = _colliders[i].transform.localScale,
                        Offset = _colliders[i].transform.localPosition
                    });

                    _colliders[i].enabled = false;
                }
            }

            PolygonCollider2D collider = _collider;
            // we set the number of paths the ship will have which is the number of the polygon colliders cached during the ship creation iterations
            collider.pathCount = colInfo.Count;

            for (int i = 0; i < colInfo.Count; i++)
            {
                // we use the scale path function before setting the path to the main ship because the wings and bodies usually have their scale change which doesn't change the scale of the collider
                collider.SetPath(i, ScalePath(colInfo[i].Polygon.GetPath(0), colInfo[i].Scale, colInfo[i].Offset));
            }
        }

        private static Vector2[] ScalePath(Vector2[] _path, Vector2 _scale, Vector2 _offset)
        {
            var scaledPath = new List<Vector2>();

            for (int i = 0; i < _path.Length; i++)
            {
                var point = _path[i];
                point.x *= _scale.x;
                point.y *= _scale.y;
                point.x += _offset.x;
                point.y += _offset.y;
                scaledPath.Add(point);
            }

            return scaledPath.ToArray();
        }
        #endregion
    }

    public static class LogsManager
    {
        public static void LogWrongType(string expectedType, string providedType)
        {
            Debug.LogError($"Expected type {expectedType}, but got {providedType} instead.");
        }
    }
}
using UnityEngine;

namespace Demo.Models
{
    public partial class CharacterSave
    {
        public class Builder
        {
            private Vector3 _position;
            private Quaternion _rotation;
            private Vector3 _scale;

            public Builder()
            {
            }

            public Builder(CharacterSave save)
            {
                _position = save.Position;
                _rotation = save.Rotation;
                _scale = save.Scale;
            }

            public Builder Position(Vector3 position)
            {
                _position = position;

                return this;
            }

            public Builder Rotation(Quaternion rotation)
            {
                _rotation = rotation;

                return this;
            }

            public Builder Scale(Vector3 scale)
            {
                _scale = scale;

                return this;
            }

            public CharacterSave Build()
            {
                return new CharacterSave()
                {
                    Position = _position,
                    Rotation = _rotation,
                    Scale = _scale
                };
            }
        }
    }

    public partial class CharacterSave
    {
        private CharacterSave()
        {
        }

        public Vector3 Position { get; private set; }

        public Quaternion Rotation { get; private set; }

        public Vector3 Scale { get; private set; }
    }
}
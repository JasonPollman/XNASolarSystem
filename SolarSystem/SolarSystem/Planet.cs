using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SolarSystem
{
    public class Planet
    {
        private int       id;
        private String    name;             // The name of the planet, for reference
        private Vector2   position;         // The X, Y postion of the planet
        private Vector2   originalPosition; // The starting position of the planet
        private Vector2   distance;         // The distance of the planet from the sun
        private Texture2D img;              // The planet image
        private float     speed;            // The revolution speed of the planet

        public Planet(int id, String name, Texture2D img, Vector2 position, float speed, Vector2 distance)
        {
            this.id       = id;
            this.name     = name;
            this.img      = img;
            this.position = originalPosition = position;
            this.speed    = speed;
            this.distance = distance;
        }

        // Setters / Getters
        public int Id { get { return id; } }
        public String Name { get { return name; } }
        public Texture2D Img { get { return img; } }

        public Vector2 Position { get { return position; } }
        public Vector2 Distance { get { return distance; } }
        public Vector2 Originalposition { get { return originalPosition; } } 

        public void setPosition(Vector2 v) { position = v; }
        public float Speed { get { return speed; } }
        


    } // End Planet class

} // End namespace SolarSystem

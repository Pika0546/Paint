﻿using Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Paint
{
    public class ShapeFactory
    {
        private List<IShape> _prototypes = new List<IShape>();
        static private ShapeFactory instance = null;
        private ShapeFactory()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string folder = Path.GetDirectoryName(exePath);
            var fis = new DirectoryInfo(folder + "\\DLL").GetFiles("*.dll");
            foreach (var f in fis)
            {
                var assembly = Assembly.Load(File.ReadAllBytes(f.FullName));
                var types = assembly.GetTypes();
                foreach (var t in types)
                {
                    if (t.IsClass && typeof(IShape).IsAssignableFrom(t))
                    {
                        IShape c = (IShape)Activator.CreateInstance(t);
                        _prototypes.Add(c);
                    }
                }
            }
        }

        static public ShapeFactory GetInstance()
        {
            if (instance == null)
            {
                instance = new ShapeFactory();
            }
            return instance;
        }

        public IShape Create(int type)
        {
            return _prototypes[type].NextShape();
        }

        public IShape Create(ShapeData data)
        {
            int totalShape = _prototypes.Count();
            IShape result = null;
            for (int i = 0; i < totalShape; i++)
            {
                if (_prototypes[i].Name == data.Name)
                {
                    result = _prototypes[i].Parse(data.Data);
                    break;
                }
            }
            return result;
        }

        public int ShapeAmount()
        {
            return _prototypes.Count();
        }
    }
}

using System;
using System.Runtime.InteropServices;

//The ray class is able to have a direction and an origin for lighting purposes

//May want to rename to hittable at some point
public abstract class Shape {
    public abstract Pixel Color();
    public abstract bool RayHit(Ray ray, double tmin, double tmax, hitRecord record);
}

public class hitRecord {
    public vec4 p;
    public vec4 normal;
    public double t;
    public bool frontFace;
    public Pixel color;

    public void SetFaceNormal(Ray ray, vec4 outwardNormal) {
        //sets normal for the record
        //outwardNormal is assumed to be unit length

        frontFace = vec4.Dot3(ray.Direction(), outwardNormal) < 0;
        normal = frontFace ? outwardNormal : vec4.Mul3(outwardNormal, -1);
    }
}

public class Sphere : Shape {
    public vec4 center;
    public double radius;
    public Pixel color;

    public Sphere(vec4 center, double radius, Pixel color) {
        this.center = center;
        this.radius = radius;
        this.color = color;
    }

    public override bool RayHit(Ray ray, double tmin, double tmax, hitRecord record) {
        //Quadractice formula
        vec4 centerMinusOrigin = vec4.Sub3(center, ray.Origin());
        double a = vec4.Dot3(ray.Direction(), ray.Direction());
        double h = vec4.Dot3(ray.Direction(), centerMinusOrigin);
        double c = vec4.Dot3(centerMinusOrigin, centerMinusOrigin) - radius * radius;
        double discriminant = h*h - a*c;

        if(discriminant < 0) {
            return false;
        }

        double sqrtDiscriminant = Math.Sqrt(discriminant);

        //find nearest root in range
        double root = (h - sqrtDiscriminant) / a;
        if(root <= tmin || tmax <= root) {
            return false;
        }

        //If this is the closest object, set the record straight
        if(root < record.t) {
            record.t = root;
            record.p = ray.Pos(record.t);
            vec4 outwardNormal = vec4.Div3(vec4.Sub3(record.p, center), radius);
            record.SetFaceNormal(ray, outwardNormal);
            record.color = this.color;
        }
        return true;

    }

    public override Pixel Color() {
        return color;
    }
}

public class Ray {
    public vec4 origin = new();
    public vec4 dir = new();
    
    public Ray() {}

    public Ray(vec4 origin, vec4 dir) {
        this.origin = origin;
        this.dir = dir;
    }

    public vec4 Pos(double t) {
        //Potential pass by reference bug where class attributes are changed
        return vec4.Add3(origin, vec4.Mul3(dir, t));
    }

    public vec4 Direction() {
        return dir;
    }

    public vec4 Origin() {
        return origin;
    }
}

//Pixel class represents the color of a pixel on a screen
public class Pixel {
    public int r;
    public int g;
    public int b;
    public vec4 rgba = new();

    public Pixel() {
        this.r = 255;
        this.g = 255;
        this.b = 255;
        rgba.e[0] = this.r;
        rgba.e[1] = this.r;
        rgba.e[2] = this.r;
        rgba.e[3] = 255;
    }

    public Pixel(int r, int g, int b) {
        this.r = r;
        this.g = g;
        this.b = b;
        rgba.e[0] = this.r;
        rgba.e[1] = this.r;
        rgba.e[2] = this.r;
        rgba.e[3] = 255;
    }
}

//vec4 Represents a 4 degree vector. In most cases 3 dimensions are all that are needed but a 4th can be
//useful sometimes
public class vec4 {
    //e is just the array of either positions or colors
    public double[] e = new double[4];
    
    public vec4() {
        e[0] = 0;
        e[1] = 0;
        e[2] = 0;
        e[3] = 0;
    }

    public vec4(double e0, double e1, double e2, double e3) {
        e[0] = e0;
        e[1] = e1;
        e[2] = e2;
        e[3] = e3;
    }

    public static double Length3(vec4 e1) {
        return Math.Sqrt(Dot3(e1, e1));
    }

    public static double Length4(vec4 e1) {
        return Math.Sqrt(Dot4(e1, e1));
    }

    //Subtraction Functions
    public static vec4 Sub3(vec4 e1, vec4 e2) {
        return new vec4(e1.e[0] - e2.e[0], e1.e[1] - e2.e[1], e1.e[2] - e2.e[2], 0);
    }

     public static vec4 Sub3(vec4 e1, double e2) {
        return new vec4(e1.e[0] - e2, e1.e[1] - e2, e1.e[2] - e2, 0);
    }

    public static vec4 Sub4(vec4 e1, vec4 e2) {
        return new vec4(e1.e[0] - e2.e[0], e1.e[1] - e2.e[1], e1.e[2] - e2.e[2], e1.e[3] - e2.e[3]);
    }

    public static vec4 Sub4(vec4 e1, double e2) {
        return new vec4(e1.e[0] - e2, e1.e[1] - e2, e1.e[2] - e2, e1.e[3] - e2);
    }

    //Addition Functions
    public static vec4 Add3(vec4 e1, vec4 e2) {
        return new vec4(e1.e[0] + e2.e[0], e1.e[1] + e2.e[1], e1.e[2] + e2.e[2], 0);
    }

     public static vec4 Add3(vec4 e1, double e2) {
        return new vec4(e1.e[0] + e2, e1.e[1] + e2, e1.e[2] + e2, 0);
    }

    public static vec4 Add4(vec4 e1, vec4 e2) {
        return new vec4(e1.e[0] + e2.e[0], e1.e[1] + e2.e[1], e1.e[2] + e2.e[2], e1.e[3] + e2.e[3]);
    }

    public static vec4 Add4(vec4 e1, double e2) {
        return new vec4(e1.e[0] + e2, e1.e[1] + e2, e1.e[2] + e2, e1.e[3] + e2);
    }


    //Multiplication Functions
    public static vec4 Mul3(vec4 e1, vec4 e2) {
        return new vec4(e1.e[0] * e2.e[0], e1.e[1] * e2.e[1], e1.e[2] * e2.e[2], 0);
    }

     public static vec4 Mul3(vec4 e1, double e2) {
        return new vec4(e1.e[0] * e2, e1.e[1] * e2, e1.e[2] * e2, 0);
    }

    public static vec4 Mul4(vec4 e1, vec4 e2) {
        return new vec4(e1.e[0] * e2.e[0], e1.e[1] * e2.e[1], e1.e[2] * e2.e[2], e1.e[3] * e2.e[3]);
    }

    public static vec4 Mul4(vec4 e1, double e2) {
        return new vec4(e1.e[0] * e2, e1.e[1] * e2, e1.e[2] * e2, e1.e[3] * e2);
    }


    //Division Functions
    
    public static vec4 Div3(vec4 e1, vec4 e2) {
        return new vec4(e1.e[0] / e2.e[0], e1.e[1] / e2.e[1], e1.e[2] / e2.e[2], 0);
    }

     public static vec4 Div3(vec4 e1, double e2) {
        return new vec4(e1.e[0] / e2, e1.e[1] / e2, e1.e[2] / e2, 0);
    }

    public static vec4 Div4(vec4 e1, vec4 e2) {
        return new vec4(e1.e[0] / e2.e[0], e1.e[1] / e2.e[1], e1.e[2] / e2.e[2], e1.e[3] / e2.e[3]);
    }

    public static vec4 Div4(vec4 e1, double e2) {
        return new vec4(e1.e[0] / e2, e1.e[1] / e2, e1.e[2] / e2, e1.e[3] / e2);
    }


    //Dot Product Functions
    public static double Dot3(vec4 e1, vec4 e2) {
        return (e1.e[0] * e2.e[0]) + (e1.e[1] * e2.e[1]) + (e1.e[2] * e2.e[2]);
    }

    public static double Dot4(vec4 e1, vec4 e2) {
        return (e1.e[0] * e2.e[0]) + (e1.e[1] * e2.e[1]) + (e1.e[2] * e2.e[2]) + (e1.e[3] * e2.e[3]);
    }


    //Cross Product Functions
    public static vec4 Cross3(vec4 e1, vec4 e2) {
        return new vec4(e1.e[1] * e2.e[2] - e1.e[2] * e2.e[1], 
        e1.e[2] * e2.e[0] - e1.e[0] * e2.e[2], 
        e1.e[0] * e2.e[1] - e1.e[1] * e2.e[0],
        0);
    }

    //Unit Vector Functions
    public static vec4 Unit3(vec4 e1) {
        double length = Length3(e1);
        return new vec4(e1.e[0] / length, e1.e[1] / length, e1.e[2] / length, 0);
    }


    public static vec4 Unit4(vec4 e1) {
        double length = Length4(e1);
        return new vec4(e1.e[0] / length, e1.e[1] / length, e1.e[2] / length, e1.e[3] / length);
    }
}

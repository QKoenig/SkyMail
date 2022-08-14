using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package
{
    public enum PackageMode
    {
        Available,
        Expired,
        Transit,
        Thrown,
        Delivered
    }

    public Package(PackageAcceptor dest, PackageMode mode, float pickupRad, float totTime, PackageCreator creator)
    {
        Destination = dest;
        Mode = mode;
        PickupRadius = pickupRad;
        TotalTime = totTime;
        Creator = creator;

        PackageManager.Instance.RegisterPackage(this);
    }

    ~Package()
    {
        PackageManager.Instance.PackageDestroyed(this);
    }

    public PackageAcceptor Destination { get; set; }
    private PackageMode _mode;
    public PackageMode Mode { get 
        {
            return _mode;
        } set
        {
            if (_mode == value)
            {
                return;
            }
            if (_mode == PackageMode.Available)
            {
                Creator.PackageTaken();
            }
            if (value == PackageMode.Transit)
            {
                PackageManager.Instance.PackagePickedUp(this);
            }
            if (value == PackageMode.Delivered)
            {
                PackageManager.Instance.PackageDelivered(this);
            }
            _mode = value;
        } 
    }

    public float PickupRadius { get; set; }
    public float StartTime { get; set; } = -1;
    public float TimeRemaining { get; set; } = -1;
    
    private float _totalTime = -1;
    public float TotalTime { get
        {
            return _totalTime;
        }
        set
        {
            TimeRemaining = _totalTime = value;
            StartTime = Time.time;
           Mode = PackageMode.Available;
        } 
    }
    public PackageCreator Creator { get; set; }

}

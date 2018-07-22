using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingBulletBehaviour : BasicBulletBehaviour
{
    private static readonly float DISCRETE_MOVEMENT_THRESHOLD = 2.0f;

    void Start()
    {
        _numSprites = BulletSprites.Length;
    }

    void Update()
    {
        //if (!_target.activeInHierarchy)
        //{
        //    _target = null;
        //}

        Vector3 bulletPos = transform.position;
        Quaternion bulletRot = transform.rotation;
        Vector3 targetPos = _target.transform.position;
        Quaternion targetRot = _target.transform.rotation;

        // Setup anguler rotation.
        Vector3 targetDir = (targetPos - bulletPos).normalized;
        int spriteIndex = SetSpriteByDirection(targetDir);
        float stepAngle = (float)spriteIndex / (float)_numSprites * 360.0f;

        float dist = Vector3.Distance(bulletPos, targetPos);
        if (dist > DISCRETE_MOVEMENT_THRESHOLD)
        {
            // Setup translational movement.
            float step = _speed * Time.deltaTime;
            //transform.position = Vector3.MoveTowards(bulletPos, targetPos, step);
            Vector3 stepDir = (Quaternion.AngleAxis(stepAngle, Vector3.forward) * Vector3.right).normalized;
            transform.position = bulletPos + (stepDir * step);
        }
        else
        {
            // Setup translational movement.
            float step = _speed * Time.deltaTime;
            //transform.position = Vector3.MoveTowards(bulletPos, targetPos, step);
            transform.position = bulletPos + (targetDir * step);
        }
	}
}

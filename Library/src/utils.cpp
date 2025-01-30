//
// Created by Carles on 18/11/2022.
//
#include "../includes/utils.hpp"
#include <algorithm>
#include <math.h>

namespace trk{
    Vector3 Vector3::zero() {
        return Vector3{0,0,0};
    }

    std::vector<float> getQuaternionFromMatrix(vr::HmdMatrix34_t mat) {
        std::vector<float> quaternion;
        float w, x, y, z;
        
        float trace = mat.m[0][0] + mat.m[1][1] + mat.m[2][2];

        if (trace > 0) {  //Its positive
            double s = 0.5 / sqrt(1.0 + trace);
            w = 0.25 / s;
            x = (mat.m[2][1] - mat.m[1][2]) * s;
            y = (mat.m[0][2] - mat.m[2][0]) * s;
            z = (mat.m[1][0] - mat.m[0][1]) * s;
        }
        else {
            if ((mat.m[0][0] > mat.m[1][1]) && (mat.m[0][0] > mat.m[2][2])) {
                float s = sqrt(1.0f + mat.m[0][0] - mat.m[1][1] - mat.m[2][2]) * 2; // s = 4 * x;
                w = (mat.m[2][1] - mat.m[1][2]) / s;
                x = 0.25f * s;
                y = (mat.m[0][1] + mat.m[1][0]) / s;
                z = (mat.m[0][2] + mat.m[2][0]) / s;
            }
            else if (mat.m[1][1] > mat.m[2][2]) {
                float s = sqrt(1.0f + mat.m[1][1] - mat.m[0][0] - mat.m[2][2]) * 2; // s = 4 * y;
                w = (mat.m[0][2] - mat.m[2][0]) / s;
                x = (mat.m[0][1] + mat.m[1][0]) / s;
                y = 0.25f * s;
                z = (mat.m[1][2] + mat.m[2][1]) / s;
            }
            else {
                float s = sqrt(1.0f + mat.m[2][2] - mat.m[0][0] - mat.m[1][1]) * 2; // s = 4 * z;
                w = (mat.m[1][0] - mat.m[0][1]) / s;
                x = (mat.m[0][2] + mat.m[2][0]) / s;
                y = (mat.m[1][2] + mat.m[2][1]) / s;
                z = 0.25f * s;
            }
        }

        float length = sqrt(w * w + x * x + y * y + z * z);
        w /= length; 		
        x /= length; 		
        y /= length; 		
        z /= length;

        quaternion.push_back(x);
        quaternion.push_back(y);
        quaternion.push_back(z);
        quaternion.push_back(w);

        return quaternion;
    }

    std::vector<float> getPosAndRotation(vr::TrackedDevicePose_t* poses, std::vector<uint32_t> trackerIndexes, bool invertX, bool invertZ, bool flipXZ ) {
        std::vector<float> positionsQuaternions;
        for (uint32_t i : trackerIndexes) {
            vr::HmdMatrix34_t mat = poses[i].mDeviceToAbsoluteTracking;
            //apply bools
            if (invertX) {
                mat.m[0][0] = -mat.m[0][0];
                mat.m[0][1] = -mat.m[0][1];
                mat.m[0][2] = -mat.m[0][2];
                mat.m[0][3] = -mat.m[0][3];
            }
            if (invertZ) {
                mat.m[2][0] = -mat.m[2][0];
                mat.m[2][1] = -mat.m[2][1];
                mat.m[2][2] = -mat.m[2][2];
                mat.m[2][3] = -mat.m[2][3];
            }
            if (flipXZ) {
                float tempXx, tempXy, tempXz, tempXw;
                tempXx = mat.m[0][0];
                tempXy = mat.m[0][1];
                tempXz = mat.m[0][2];
                tempXw = mat.m[0][3];

                mat.m[0][0] = mat.m[2][0];
                mat.m[0][1] = mat.m[2][1];
                mat.m[0][2] = mat.m[2][2];
                mat.m[0][3] = mat.m[2][3];

                mat.m[2][0] = tempXx;
                mat.m[2][1] = tempXy;
                mat.m[2][2] = tempXz;
                mat.m[2][3] = tempXw;
            }

            std::vector<float> quat = getQuaternionFromMatrix(mat);
            positionsQuaternions.push_back(mat.m[0][3]);
            positionsQuaternions.push_back(mat.m[1][3]);
            positionsQuaternions.push_back(mat.m[2][3]);
            positionsQuaternions.push_back(quat[0]);
            positionsQuaternions.push_back(quat[1]);
            positionsQuaternions.push_back(quat[2]);
            positionsQuaternions.push_back(quat[3]);
        }
        return positionsQuaternions;
    }
}
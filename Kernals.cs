using System;
using System.Collections.Generic;
using System.Text;

namespace monoSlime2
{
    public class Kernals
    {

       public static string draw = @"
                   int Abgr_packedint2(int a, int b, int g, int r)
                        {
                            int Ccolor = 0;
                            Ccolor |= a << 24;
                            Ccolor |= b << 16;
                            Ccolor |= g << 8;
                            Ccolor |= r;
                            return Ccolor;
                        }
                    
                   __kernel void
                    draw(      __global       int * img_rgb_PackedInts,
                               __global       int * img_rgb_ValuesInts,
                               __global       int * img_trails_rgb_ValuesInts,
                               __global       int * img_pixel_RgbValueStartIndex,
                               __global       float * agent_pos_x,
                               __global       float * agent_pos_y,
                               __global       float * agent_bearing,
                               __global       float * agent_mag,
                               __global       int * agent_pixelID,
                               __global       int * imageInfoArray,
                               __global       float * settingArray, // 0_turnStrength(0-180)
                               __global       float * debugOuts
                            
                                    ) 
                {
                            int id = get_global_id(0);

                             img_rgb_PackedInts[id] = 0;
                            int pxStart = img_pixel_RgbValueStartIndex[id];
                            int a = img_trails_rgb_ValuesInts[pxStart] ;
                            int b = img_trails_rgb_ValuesInts[pxStart+1] ;
                            int g = img_trails_rgb_ValuesInts[pxStart+2] ;
                            int r = img_trails_rgb_ValuesInts[pxStart+3] ;
                            img_rgb_PackedInts[id] = Abgr_packedint2(a, b, g, r);
                            
                            a = img_rgb_ValuesInts[pxStart] ;
                            b = img_rgb_ValuesInts[pxStart+1] ;
                            g = img_rgb_ValuesInts[pxStart+2] ;
                            r = img_rgb_ValuesInts[pxStart+3] ;
                            int sum = (b+g+r);
                            if(sum > 0) {
                            img_rgb_PackedInts[id] = Abgr_packedint2(a, b, g, r);
                            }

                }";

       public static string blurr = @"
                   int Abgr_packedint3(int a, int b, int g, int r)
                        {
                            int Ccolor = 0;
                            Ccolor |= a << 24;
                            Ccolor |= b << 16;
                            Ccolor |= g << 8;
                            Ccolor |= r;
                            return Ccolor;
                        }
                    
                   __kernel void
                    blurr(      __global       int * img_rgb_PackedInts,
                               __global       int * img_rgb_ValuesInts,
                               __global       int * img_trails_rgb_ValuesInts,
                               __global       int * img_pixel_RgbValueStartIndex,
                               __global       float * agent_pos_x,
                               __global       float * agent_pos_y,
                               __global       float * agent_bearing,
                               __global       float * agent_mag,
                               __global       int * agent_pixelID,
                               __global       int * imageInfoArray,
                               __global       float * settingArray, // 0_turnStrength(0-180)
                               __global       float * debugOuts
                            
                                    ) 
                {
                            int id = get_global_id(0);

                            
                            int neighborsCount = 1;  

                            //This Pixel
                            int pxStart = img_pixel_RgbValueStartIndex[id];
                            int sumR = (int)(img_trails_rgb_ValuesInts[pxStart+3] * 0.25);
                            int sumG = (int)(img_trails_rgb_ValuesInts[pxStart+2] * 0.25);
                            int sumB = (int)(img_trails_rgb_ValuesInts[pxStart+1] * 0.25);

                            //neighbor pixels
                            int totalPixels = imageInfoArray[0]*imageInfoArray[1];
                            int thisPixelID = id;
                            int leftPixelID = id - 1;
                            int rightPixelID = id + 1;
                            int upleftPixelID = id - imageInfoArray[0] - 1;
                            int upPixelID = id - imageInfoArray[0];
                            int uprightPixelID = id - imageInfoArray[0] + 1;
                            int downleftPixelID = id + imageInfoArray[0] -1;
                            int downPixelID = id + imageInfoArray[0];
                            int downrightPixelID = id + imageInfoArray[0] +1;
                            if (leftPixelID >= 0 && leftPixelID < totalPixels) { 
                               neighborsCount++; 
                               sumR += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[leftPixelID]+3] * 0.125); 
                               sumG += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[leftPixelID]+2] * 0.125); 
                               sumB += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[leftPixelID]+1] * 0.125);
                            }
                            if (rightPixelID >= 0 && rightPixelID < totalPixels) { 
                               neighborsCount++; 
                               sumR += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[rightPixelID]+3] * 0.125);
                               sumG += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[rightPixelID]+2] * 0.125);
                               sumB += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[rightPixelID]+1] * 0.125);
                            }
                            if (upleftPixelID >= 0 && upleftPixelID < totalPixels) {
                               neighborsCount++;
                               sumR += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[upleftPixelID]+3] * 0.0624);
                               sumG += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[upleftPixelID]+2] * 0.0624);
                               sumB += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[upleftPixelID]+1] * 0.0624);
                            }
                            if (upPixelID >= 0 && upPixelID < totalPixels) {
                               neighborsCount++;
                               sumR += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[upPixelID]+3] * 0.125);
                               sumG += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[upPixelID]+2] * 0.125);
                               sumB += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[upPixelID]+1] * 0.125);
                            }
                            if (uprightPixelID >= 0 && uprightPixelID < totalPixels) {
                               neighborsCount++;
                               sumR += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[uprightPixelID]+3] * 0.0624);
                               sumG += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[uprightPixelID]+2] * 0.0624);
                               sumB += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[uprightPixelID]+1] * 0.0624);
                            }
                            if (downleftPixelID >= 0 && downleftPixelID < totalPixels) { 
                               neighborsCount++; 
                               sumR += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[downleftPixelID]+3] * 0.0624); 
                               sumG += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[downleftPixelID]+2] * 0.0624); 
                               sumB += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[downleftPixelID]+1] * 0.0624);
                               }
                            if (downPixelID >= 0 && downPixelID < totalPixels) { 
                               neighborsCount++; 
                               sumR += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[downPixelID]+3] * 0.125);
                               sumG += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[downPixelID]+2] * 0.125);
                               sumB += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[downPixelID]+1] * 0.125);
                            }
                            if (downrightPixelID >= 0 && downrightPixelID < totalPixels) {
                               neighborsCount++;
                               sumR += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[downrightPixelID]+3] * 0.0624);
                               sumG += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[downrightPixelID]+2] * 0.0624);
                               sumB += (int)(img_trails_rgb_ValuesInts[img_pixel_RgbValueStartIndex[downrightPixelID]+1] * 0.0624);
                            }
                            
                            int sumtotal = sumR + sumG + sumB;
                            int outR = sumR;//(int)(sumR / neighborsCount);
                            int outG = sumG;//(int)(sumG / neighborsCount);
                            int outB = sumB;//(int)(sumB / neighborsCount);
                            
                            //int thispixelrgbid = rgbids[thisPixelID];
                            img_trails_rgb_ValuesInts[pxStart] = 0;
                            img_trails_rgb_ValuesInts[pxStart + 1] = outB;
                            img_trails_rgb_ValuesInts[pxStart + 2] = outG;
                            img_trails_rgb_ValuesInts[pxStart + 3] = outR;

                            
                      

                }";

       public static string test = @"
                    uint hash(uint state) {
                        state ^= 2747636419u;
                        state *= 2654435769u;
                        state ^= state >> 16;
                        state *= 2654435769u;
                        state ^= state >> 16;
                        state *= 2654435769u;
                        return state;                  
                    }
                    // max hash value = 4294967295.0;                    

                    double makebearing(double a1, double a2, double b1, double b2) {
                                 double TWOPI = 6.2831853071795865;
                                 double RAD2DEG = 57.2957795130823209;
                                //  if (a1 = b1 and a2 = b2) throw an error 
                                  double theta = atan2(b1 - a1, a2 - b2);
                                 if (theta < 0.0) {
                                       theta += TWOPI;
                                  }
                                  double r = RAD2DEG * theta;
                         //        double test = a1 * a2;
                                  return r;
                                 }
                    
                    float xVector(float angle, float magnitude) {
                            float piR = 3.14159265359 /180;
                            float x = 0;
                            float fixedAngle = 0;
                            
                            if (angle >= 0 && angle < 90 ) {
                                float rad = (angle * (piR));
                                x = magnitude * sin(rad);
                                x = fabs(x);
                                //x *= -1;
                            }
                            if (angle >= 90 && angle < 180 ) {
                                fixedAngle = angle - 90;
                                float rad = (fixedAngle * (piR));
                                x = (magnitude * cos(rad));
                                x = fabs(x);
                              //  x *= -1;
                            }
                            if (angle >= 180 && angle < 270 ) {
                                fixedAngle = angle - 180;
                                float rad = (fixedAngle * (piR));
                                x = magnitude * sin(rad);
                                x = fabs(x);
                                x *= -1;
                            }
                            if (angle >= 270) {
                                fixedAngle = angle - 270;
                                float rad = (fixedAngle * (piR));
                                x = magnitude * cos(rad);
                                x = fabs(x);
                                x *= -1;
                            }
                            return x;
                    }
                    
                    float yVector(float angle, float magnitude) {
                            float piR = 3.14159265359 /180;
                            float y = 0;
                            float fixedAngle = 0;
                            
                            if (angle >= 0 && angle < 90 ) {
                                float rad = (angle * (piR));
                                y = magnitude * cos(rad);
                                y = fabs(y);
                                y *= -1;
                            }
                            if (angle >= 90 && angle < 180 ) {
                                fixedAngle = angle - 90;
                                float rad = (fixedAngle * (piR));
                                y = (magnitude * sin(rad));
                                y = fabs(y);
                                //y *= -1;
                            }
                            if (angle >= 180 && angle < 270 ) {
                                fixedAngle = angle - 180;
                                float rad = (fixedAngle * (piR));
                                y = magnitude * cos(rad);
                                y = fabs(y);
                                //y *= -1;
                            }
                            if (angle >= 270) {
                                fixedAngle = angle - 270;
                                float rad = (fixedAngle * (piR));
                                y = magnitude * sin(rad);
                                y = fabs(y);
                                y *= -1;
                            }
                            return y;
                    }

                    int Abgr_packedint(int a, int b, int g, int r)
                        {
                            int Ccolor = 0;
                            Ccolor |= a << 24;
                            Ccolor |= b << 16;
                            Ccolor |= g << 8;
                            Ccolor |= r;
                            return Ccolor;
                        }

                     __kernel void
                    Test(      __global       int * img_rgb_PackedInts,
                               __global       int * img_rgb_ValuesInts,
                               __global       int * img_trails_rgb_ValuesInts,
                               __global       int * img_pixel_RgbValueStartIndex,
                               __global       float * agent_pos_x,
                               __global       float * agent_pos_y,
                               __global       float * agent_bearing,
                               __global       float * agent_mag,
                               __global       int * agent_pixelID,
                               __global       int * imageInfoArray,
                               __global       float * settingArray, // 0_turnStrength(0-180) 1_sensorangle
                               __global       float * debugOuts
                            
                                    )
                            {
                                int id = get_global_id(0);
                                int cR = 0;
                                int cG = 0;
                                int cB = 255;

                            
                                //Clear last spot
                                 int px = agent_pixelID[id];
                                 int lastpixelRgbIndex = img_pixel_RgbValueStartIndex[px];
                                 
                                    
                                // img_rgb_PackedInts[px] = Abgr_packedint(img_rgb_ValuesInts[pixelRgbIndex], img_rgb_ValuesInts[pixelRgbIndex +1],img_rgb_ValuesInts[pixelRgbIndex +2], img_rgb_ValuesInts[pixelRgbIndex +3]);
                                    
                                
                            //Calc vectors
                                float xV = xVector(agent_bearing[id], agent_mag[id]);
                                float yV = yVector(agent_bearing[id], agent_mag[id]);
                            
                            // turn at walls
                                if ( (agent_pos_x[id] >= imageInfoArray[0]-10) || (agent_pos_x[id] <= 10) ) { 
                                            xV *= -1;
                                      if (agent_pos_x[id] >= imageInfoArray[0]-10) { agent_pos_x[id] = imageInfoArray[0]-11; }
                                      else { agent_pos_x[id] = 11;}
                                    }
                                if ( (agent_pos_y[id] >= imageInfoArray[1]-10) || (agent_pos_y[id] <= 10) ) { 
                                            yV *= -1;
                                      if (agent_pos_y[id] >= imageInfoArray[1]-10) { agent_pos_y[id] = imageInfoArray[1]-11; }
                                      else { agent_pos_y[id] = 11;}
                                    }
                            
                            //make new bearing
                                agent_bearing[id] = (float)makebearing(agent_pos_x[id], agent_pos_y[id], (agent_pos_x[id] + xV), (agent_pos_y[id] + yV));
                            
                            //Calc vectors
                                xV = xVector(agent_bearing[id], agent_mag[id]);
                                yV = yVector(agent_bearing[id], agent_mag[id]);
                            
                            //Build sensors pixel ids;
                                //front sensor pos
                                bool sensor1_inbounds = true;
                                int sens1x = (int)(agent_pos_x[id] + (xV*9 ));
                                int sens1y = (int)(agent_pos_y[id] + (yV*9));
                                if (sens1x < 1 || sens1x > imageInfoArray[0] -1) { sensor1_inbounds = false; }
                                if (sens1y < 1 || sens1y > imageInfoArray[1] -1) { sensor1_inbounds = false; }

                                //left sensor
                                bool sensor2_inbounds = true;
                                float bearing_to_leftsensor = agent_bearing[id] - settingArray[1];
                                if (bearing_to_leftsensor < 0) { bearing_to_leftsensor = 360 - fabs(bearing_to_leftsensor); }
                                float x_to_sens2 = xVector(bearing_to_leftsensor, 9);
                                float y_to_sens2 = yVector(bearing_to_leftsensor, 9);
                                int sens2x = round(agent_pos_x[id] + x_to_sens2);
                                int sens2y = round(agent_pos_y[id] + y_to_sens2);
                                if (sens2x < 1 || sens2x > imageInfoArray[0] -1) { sensor2_inbounds = false; }
                                if (sens2y < 1 || sens2y > imageInfoArray[1] -1) { sensor2_inbounds = false; }
                                
                                //right sensor
                                bool sensor3_inbounds = true;
                                float bearing_to_rightsensor = agent_bearing[id] + settingArray[1];                               
                                if (bearing_to_rightsensor >= 360) { bearing_to_rightsensor = fabs(bearing_to_rightsensor) - 360; }
                                float x_to_sens3 = xVector(bearing_to_rightsensor, 9);
                                float y_to_sens3 = yVector(bearing_to_rightsensor, 9);
                                int sens3x = agent_pos_x[id] + x_to_sens3;
                                int sens3y = agent_pos_y[id] + y_to_sens3;
                                if (sens3x < 1 || sens3x > imageInfoArray[0] -1) { sensor3_inbounds = false; }
                                if (sens3y < 1 || sens3y > imageInfoArray[1] -1) { sensor3_inbounds = false; }

                                int s1PixelID = ((imageInfoArray[0] * sens1y) + sens1x);
                                int s2PixelID = ((imageInfoArray[0] * sens2y) + sens2x);
                                int s3PixelID = ((imageInfoArray[0] * sens3y) + sens3x);
                                
                            //Move
                                float targetx = agent_pos_x[id] + xV;   
                                float targety = agent_pos_y[id] + yV;
                                int v = (int)((imageInfoArray[0] * round(targety)) + round(targetx));
                                int targetpxRGBstart = img_pixel_RgbValueStartIndex[v];// 
                                int  target_rgbSUM = 0;
                                if (v != agent_pixelID[id]) {
                                target_rgbSUM = img_rgb_ValuesInts[targetpxRGBstart +1] + img_rgb_ValuesInts[targetpxRGBstart +2] + img_rgb_ValuesInts[targetpxRGBstart +3];  
                                } 
                                bool move = false;
                                uint rn = 0;
                                debugOuts[4] = target_rgbSUM;
                                debugOuts[5] = targetx;
                                debugOuts[6] = targety;
                                debugOuts[7] = agent_pos_x[id];
                                debugOuts[8] = agent_pos_y[id];
                                debugOuts[9] = v;
                                debugOuts[10] = agent_pixelID[id];
                                
                                int sensor1_stength = 0;
                                int sensor2_stength = 0;
                                int sensor3_stength = 0;
                                if (true) { // target_rgbSUM == 0
                                  move = true;
                                  agent_pos_x[id] += xV;   
                                  agent_pos_y[id] += yV;   
                                  agent_pixelID[id] = (imageInfoArray[0] * round(agent_pos_y[id]) + round(agent_pos_x[id]));
                                
                                  
                                    
                                    
                                    
                                  int s1_pixelRgbIndex = img_pixel_RgbValueStartIndex[s1PixelID];
                                  int s2_pixelRgbIndex = img_pixel_RgbValueStartIndex[s2PixelID];
                                  int s3_pixelRgbIndex = img_pixel_RgbValueStartIndex[s3PixelID];

                                  if (sensor1_inbounds) {                                
                                  //sensor1_stength += img_trails_rgb_ValuesInts[s1_pixelRgbIndex];
                                  sensor1_stength += img_trails_rgb_ValuesInts[s1_pixelRgbIndex +1];
                                  sensor1_stength += img_trails_rgb_ValuesInts[s1_pixelRgbIndex +2];
                                  sensor1_stength += img_trails_rgb_ValuesInts[s1_pixelRgbIndex +3];
                                  debugOuts[1] = sensor1_stength;
                                  }
                                  if (sensor2_inbounds) {
                                  //sensor2_stength += img_trails_rgb_ValuesInts[s2_pixelRgbIndex];
                                  sensor2_stength += img_trails_rgb_ValuesInts[s2_pixelRgbIndex +1];
                                  sensor2_stength += img_trails_rgb_ValuesInts[s2_pixelRgbIndex +2];
                                  sensor2_stength += img_trails_rgb_ValuesInts[s2_pixelRgbIndex +3];
                                  debugOuts[2] = sensor2_stength;
                                  }
                                  if (sensor3_inbounds) {
                                  //sensor3_stength += img_trails_rgb_ValuesInts[s3_pixelRgbIndex] ;
                                  sensor3_stength += img_trails_rgb_ValuesInts[s3_pixelRgbIndex +1] ;
                                  sensor3_stength += img_trails_rgb_ValuesInts[s3_pixelRgbIndex +2] ;
                                  sensor3_stength += img_trails_rgb_ValuesInts[s3_pixelRgbIndex +3] ;
                                  debugOuts[3] = sensor3_stength;
                                  }
                                  bool notForward = true;
                                  if (sensor1_stength > sensor2_stength && sensor1_stength > sensor3_stength) {
                                            //do nothing
                                            notForward = false;
                                  }
                                  
                                  if (sensor2_stength > sensor3_stength && notForward == true) { //sensor2_stength > sensor3_stength
                                           // float str = ((sensor2_stength - sensor3_stength) / 765) * 10;
                                            float newBearing = agent_bearing[id] - settingArray[0];
                                            if (newBearing < 0) { newBearing = (360 + newBearing);} 
                                            agent_bearing[id] = newBearing;
                                           
                                  }
                                  if (sensor3_stength > sensor2_stength && notForward == true)  { //sensor2_stength < sensor3_stength
                                          //  float str = ((sensor3_stength - sensor2_stength) / 765) * 4;
                                            float newBearing = agent_bearing[id] + settingArray[0];
                                            if (newBearing >= 360) { newBearing = newBearing - 360;}
                                            agent_bearing[id] = newBearing;
                                  
                                    }
                                  
                                  if (sensor2_stength == sensor3_stength && notForward == true)  { 
                                           uint r = hash(agent_pos_x[id] + agent_pos_y[id] * id);
                                           rn = hash(agent_pos_x[id] + agent_pos_y[id] * id);
                                           float zero_one = (r / 4294967295.0);
                                           //debugData[4] += zero_one;
                                           float str = (rn / 4294967295.0) * 4;
                                           if (zero_one > 0.7) { str *= -1;}
                                           agent_bearing[id] += (int)str;
                                           if (agent_bearing[id] >= 360) { agent_bearing[id] = agent_bearing[id] - 360;}
                                           if (agent_bearing[id] < 0) { agent_bearing[id] = 360 - fabs(agent_bearing[id]);} 
                                    }
                                
                                    
                                } else {
                                    //make new bearing
                                      uint rand0_1 = (hash(agent_bearing[id]) / 4294967295.0);
                                      uint rr = hash((uint)(agent_bearing[id]+agent_pixelID[id]+id));
                                      rr = hash(rr);
                                      
                                      float rand0_360 = (hash(rr) / 4294967295.0) * 360;
                                        
                                      agent_bearing[id] = rand0_360;
                                     // agent_bearing[id] += 180;
                                       
                                        
                                      
                                      if (agent_bearing[id] >= 360) { agent_bearing[id] = agent_bearing[id] - 360;}
                                      if (agent_bearing[id] < 0) { agent_bearing[id] = 360 - fabs(agent_bearing[id]);} 
                                }
                                  debugOuts[0] = agent_bearing[id];
                              //    debugOuts[8] = sensor1_stength;
                              //    debugOuts[9] = sensor2_stength;
                              //    debugOuts[10] = sensor3_stength;
                            
                           //Paint
                           //clear last
                                
                           //addto trails
                                 if (move) {
                                   img_trails_rgb_ValuesInts[lastpixelRgbIndex] = 255; //img_rgb_ValuesInts[pixelRgbIndex];
                                   img_trails_rgb_ValuesInts[lastpixelRgbIndex + 1] = cB;//img_rgb_ValuesInts[pixelRgbIndex +1];
                                   img_trails_rgb_ValuesInts[lastpixelRgbIndex + 2] = cG;//img_rgb_ValuesInts[pixelRgbIndex +2];
                                   img_trails_rgb_ValuesInts[lastpixelRgbIndex + 3] = cR;//img_rgb_ValuesInts[pixelRgbIndex +3];
                                 }
                                 img_rgb_ValuesInts[lastpixelRgbIndex] = 255;
                                 img_rgb_ValuesInts[lastpixelRgbIndex +1] = 0;
                                 img_rgb_ValuesInts[lastpixelRgbIndex +2] = 0;
                                 img_rgb_ValuesInts[lastpixelRgbIndex +3] = 0;
                          //paint agent    
                                 px = agent_pixelID[id];
                                 int pixelRgbIndex = img_pixel_RgbValueStartIndex[px];
                                 
                                 img_rgb_ValuesInts[pixelRgbIndex] = 255;
                                 img_rgb_ValuesInts[pixelRgbIndex +1] = cB;
                                 img_rgb_ValuesInts[pixelRgbIndex +2] = cG;
                                 img_rgb_ValuesInts[pixelRgbIndex +3] = cR;
                      
                           //paint sensors
                          //      int s1_pixelRgbIndex = img_pixel_RgbValueStartIndex[s1PixelID];
                          //      int s2_pixelRgbIndex = img_pixel_RgbValueStartIndex[s2PixelID];
                          //      int s3_pixelRgbIndex = img_pixel_RgbValueStartIndex[s3PixelID];
                          //       if (sensor1_inbounds) {
                          //        img_rgb_ValuesInts[s1_pixelRgbIndex] = 255;
                         //         img_rgb_ValuesInts[s1_pixelRgbIndex +1] = 255;
                          //        img_rgb_ValuesInts[s1_pixelRgbIndex +2] = 255;
                          //        img_rgb_ValuesInts[s1_pixelRgbIndex +3] = 255;
                          //      }
                          //      if (sensor2_inbounds) {
                          //      img_rgb_ValuesInts[s2_pixelRgbIndex] = 255;
                          //      img_rgb_ValuesInts[s2_pixelRgbIndex +1] = 0;
                          //      img_rgb_ValuesInts[s2_pixelRgbIndex +2] = 255;
                          //      img_rgb_ValuesInts[s2_pixelRgbIndex +3] = 255;
                          //      }
                         //       if (sensor3_inbounds) {
                          //      img_rgb_ValuesInts[s3_pixelRgbIndex] = 255;
                          //      img_rgb_ValuesInts[s3_pixelRgbIndex +1] = 255;
                          //      img_rgb_ValuesInts[s3_pixelRgbIndex +2] = 0;
                          //      img_rgb_ValuesInts[s3_pixelRgbIndex +3] = 0;
                          //      }

                                
                            //    img_rgb_PackedInts[px] = Abgr_packedint(img_rgb_ValuesInts[pixelRgbIndex], img_rgb_ValuesInts[pixelRgbIndex +1],img_rgb_ValuesInts[pixelRgbIndex +2], img_rgb_ValuesInts[pixelRgbIndex +3]);
                            //    img_rgb_PackedInts[s1PixelID] = Abgr_packedint(img_rgb_ValuesInts[s1_pixelRgbIndex], img_rgb_ValuesInts[s1_pixelRgbIndex +1],img_rgb_ValuesInts[s1_pixelRgbIndex +2], img_rgb_ValuesInts[s1_pixelRgbIndex +3]);
                             //   img_rgb_PackedInts[s2PixelID] = Abgr_packedint(img_rgb_ValuesInts[s2_pixelRgbIndex], img_rgb_ValuesInts[s2_pixelRgbIndex +1],img_rgb_ValuesInts[s2_pixelRgbIndex +2], img_rgb_ValuesInts[s2_pixelRgbIndex +3]);
                             //   img_rgb_PackedInts[s3PixelID] = Abgr_packedint(img_rgb_ValuesInts[s3_pixelRgbIndex], img_rgb_ValuesInts[s3_pixelRgbIndex +1],img_rgb_ValuesInts[s3_pixelRgbIndex +2], img_rgb_ValuesInts[s3_pixelRgbIndex +3]);
                            }";

    }
}

# Educational Ransomware Project

## Overview
This project is a part of a cybersecurity research initiative aimed at understanding the workings of antivirus response to ransomware and developing strategies to mitigate its effects. The project will encrypt files and should be used for educational and research purposes only.

## Disclaimer
This code is intended for educational and research use only. The authors do not condone illegal activity. Any actions and or activities related to the material contained within this repository is solely your responsibility. Misuse of the information in this project can result in criminal charges brought against the persons in question. The authors will not be held responsible in the event any criminal charges be brought against any individuals misusing the information in this repository to break the law.

## Project Description
The ransomware is a .NET application that encrypts files by iterating through a users directories. This project includes tools for encryption and decryption, demonstrating the full life cycle of a ransomware attack and decryption from a mock C2 server.

## Features
- Encrypt files with random IV and key
- Decryption process triggered by mock C2 server (MySQL)
- Records computers MAC address to SQL server
- Marks process as OS critical causing BSOD on killing of process

## Installation
1. Clone the repository to your local machine using `git clone`.
2. Compile the project using Visual Studio or any compatible .NET compiler.
3. Setup an SQL server with the appropriate tables and columns

## Usage
This application should be run in a controlled environment where you have permissions to perform such activities. 

## Research Findings
Our findings are documented in the research paper associated with this project. Key takeaways include the importance of cybersecurity measures beyond conventional antivirus software, and recommendations for user education and system configuration.


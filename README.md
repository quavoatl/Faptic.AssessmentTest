# Faptic assessment solution (.NET 7 + InMemoryDatabase)

**Installation steps:**
- git clone [https://github.com/quavoatl/Faptic.AssessmentTest](https://github.com/quavoatl/Faptic.AssessmentTest.git)
- git checkout master

**Running steps:**
- provided solution runs by a Dockerfile
- cd directory where Dockerfile is located
- open powershell
- run command: docker build -f "PathToDockerfile\Dockerfile" -t assessment:dev "PathToSlnFile"
- example: docker build -f "C:\Users\User1\source\repos\Assessment\Dockerfile" -t assessment:dev "C:\Users\User1\source\repos\Assessment"
- run command: docker run --publish 5000:80 assessment:dev
- example: docker run --publish 5000:80 assessment:dev
  
**Easy test values**
- run multiple POST /api/reporting requests with startPoint: 1672531200, 1672534800, 1672538400, 1672542000 to feed some data

**TESTS COVERAGE**

- ![alt text](https://i.imgur.com/dIJZJ8D.png)



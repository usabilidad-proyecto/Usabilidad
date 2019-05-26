timestamp=$(date +%s)
echo Enter your Game filename.exe: 
read GAME_NAME
start Proyecto-Grupo03.exe Profiler "$timestamp"

while [ ! -f Profiler.txt ] ;
do
      sleep 1
done
ProccessName=`cat Profiler.txt`
start HW_Profiler.exe "$ProccessName" "$timestamp"
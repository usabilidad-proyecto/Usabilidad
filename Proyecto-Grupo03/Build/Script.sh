timestamp=$(date +%s)
echo "$timestamp"
echo Enter your Game filename.exe: 
read GAME_NAME
start $GAME_NAME Profiler "$timestamp"

while [ ! -f Profiler.txt ] ;
do
      sleep 1
done
ProccessName=`cat Profiler.txt`
echo "$ProccessName"

start HW_Profiler.exe "$ProccessName" "$timestamp"
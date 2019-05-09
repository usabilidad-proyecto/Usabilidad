

echo Enter your Game filename.exe: 
read GAME_NAME
start $GAME_NAME Profiler



while [ ! -f Profiler.txt ] ;
do
      sleep 1
done
ProccessName=`cat Profiler.txt`
echo "$ProccessName"


start ProfilerExterno.exe %ProccessName%


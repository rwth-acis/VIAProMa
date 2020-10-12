#for Var in {0..62}
#do
#    echo "      curvePoint"$Var"[index] = curve["$Var"];"
#done

#for Var in {0..62}
#do
#    echo "      curve["$Var"] = curvePoint"$Var"[index];"
#done

#for Var in {0..62}
#do
#    echo "      jobData.curvePoint"$Var" = new NativeArray<Vector3>(count, Allocator.TempJob);"
#done

for Var in {0..62}
do
    echo "      jobData.curvePoint"$Var".Dispose();"
done
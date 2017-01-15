#coding=utf-8
# refer:http://zrj.me/archives/1186
import sys

source_files = sys.argv[1:-1]
dest_file = sys.argv[-1]

print "要进行拼接的源文件: " + (",").join(source_files)
print "拼接之后的文件保存为: " + dest_file
ensure = raw_input("是否继续?(y/n): ")
# ensure = 'y' #TODO
if ensure is 'n':
	print "已退出拼接进程."
	exit()
elif ensure is 'y':
	line_num = 0
	oup_file = open(dest_file, "w+")

	for source_file in source_files:
		file = open(source_file)
		source_data = file.read()
		oup_file.write(source_data)
		file.close()

	oup_file.close()
	line_num = len(open(dest_file).readlines())
	print "拼接的数据总行数为: " + str(line_num) 
else:
	print "无效输入."
	exit()

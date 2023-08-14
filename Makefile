run:
	sudo docker build --rm -t satellites-image -f  Satellites/Dockerfile .
	sudo docker run -it --rm satellites-image 


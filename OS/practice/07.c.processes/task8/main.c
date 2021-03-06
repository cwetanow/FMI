/*
Да се напише програма на С, която получава като параметър име на файл. Създава процес син, който записва стринга foobar във файла (ако не съществува, го създава, в противен случай го занулява), след което процеса родител прочита записаното във файла съдържание и го извежда на стандартния изход, добавяйки по един интервал между всеки два символа.
*/

#include <stdio.h>
#include <err.h>
#include <fcntl.h>
#include <stdlib.h>
#include <unistd.h>

#include <sys/types.h>
#include <sys/wait.h>

int main(int argc, char *argv[])
{
	if (argc != 2)
	{
		exit(-1);
	}

	int fd;
	if ((fd = open(argv[1], O_CREAT | O_WRONLY)) == -1)
	{
		write(2, "Err", 4);
	}

	int status;

	if (fork() > 0)
	{
		wait(&status);

		fd = open(argv[1], O_RDONLY);

		char c;
		int i = 0;

		while (read(fd, &c, 1))
		{
			if (i == 2)
			{
				write(1, " ", 1);
				i = 0;
			}

			i++;
			write(1, &c, 1);
		}
	}
	else
	{
		write(fd, "foobar", sizeof("foobar"));
		close(fd);
	}

	exit(0);
}

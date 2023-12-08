CC := gcc
CFLAGS := -Wall -g  # example C flags, adjust as necessary
TARGET := bytecode  # change to your desired output name
SOURCES := $(wildcard *.c)
OBJECTS := $(SOURCES:.c=.o)

$(TARGET): $(OBJECTS)
	$(CC) $(CFLAGS) -o $(TARGET) $(OBJECTS)

%.o: %.c
	$(CC) $(CFLAGS) -c $<

clean:
	rm -f $(OBJECTS) $(TARGET)

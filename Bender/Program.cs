using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

class Solution
{
    static void Main(string[] args)
    {
        string[] inputs = Console.ReadLine().Split(' ');
        int L = int.Parse(inputs[0]);
        int C = int.Parse(inputs[1]);
        char[,] world = new char[L, C];
        (int, int) pos = (0, 0);
        for (int i = 0; i < L; i++)
        {
            string row = Console.ReadLine();
            for (int j = 0; j < C; j++)
            {
                world[i, j] = row[j];
                if (row[j] == '@')
                    pos = (i, j);
            }
        }

        Game bender = new Game(world, pos, (L, C));
        bender.StartGame();
    }
}

public class Game
{

    private char[,] world = null;
    private char[,] renderWorld = null;

    private (int, int) position = (0, 0);
    private (int, int) direction = (1, 0);
    private enum directions{
        SOUTH,
        EAST,
        NORTH,
        WEST
    }
    private directions currentDirection = directions.SOUTH;
    private (int, int) size = (0, 0);
    private ((int, int), (int, int)) teleports = ((0, 0), (0, 0));

    private bool IsBeerMaster = false;
    private bool IsGame = true;
    private bool Prioriters = false;
    public Game(char[,] world, (int, int) BenderPos, (int, int) size)
    {
        position = BenderPos;
        this.world = world;
        this.world[position.Item1, position.Item2] = ' ';
        this.world[size.Item1 - 1, size.Item2 - 1] = '#';
        this.size = size;
        renderWorld = (char[,])world.Clone();
        FindTeleports();
    }
    private void FindTeleports()
    {
        for(int i = 0; i < size.Item1; i++)
        {
            for (int j = 0; j < size.Item2; j++)
            {
                if (world[i, j] == 'T' && teleports.Item1 == (0,0))
                    teleports.Item1 = (i, j);
                else if(world[i, j] == 'T')
                    teleports.Item2 = (i, j);
            }
        }
    }
    public void StartGame()
    {
        while (IsGame)
        {
            UpdateBenderLogic();
            DrawMaze();
            Console.WriteLine(currentDirection);
            System.Threading.Thread.Sleep(1000);
        }
    }
    private void DrawMaze()
    {
        for(int i = 0; i < size.Item1; i++)
        {
            for (int j = 0; j < size.Item2; j++)
            {
                if((i,j) == position) 
                    Console.Write("@");
                else if((i, j) == teleports.Item1 || (i, j) == teleports.Item2) 
                    Console.Write("T");
                else 
                    Console.Write(world[i, j]);
            }
            Console.WriteLine();
        }
    }
    private void UpdateBenderLogic()
    {
        (int, int) position2 = (position.Item1 + direction.Item1, position.Item2 + direction.Item2);
        if (world[position2.Item1, position2.Item2] != '#')
            CheckOnEffect(position2, world[position2.Item1, position2.Item2]);
        else
            ChangeDirection();
    }
    private void CheckOnEffect((int, int) position, char cell)
    {
        if (cell != ' ')
        {
            if (cell == 'X' && !IsBeerMaster)
            {
                ChangeDirection();
                return;
            }
            Move(position);
            if (cell == 'B')
                IsBeerMaster = !IsBeerMaster;
            if (cell == 'E')
                currentDirection = directions.EAST;

            if (cell == 'W')
                currentDirection = directions.WEST;

            if (cell == 'N')
                currentDirection = directions.NORTH;

            if (cell == 'S')
                currentDirection = directions.SOUTH;

            if (cell == 'I')
                Inverse();

            if (cell == 'T')
            {
                Teleport(position);
                return;
            }
            if (cell == '$')
            {
                IsGame = false;
                return;
            }
            SetDirection();
        }
        else
            Move(position);
    }
    private void Inverse()
    {
        Prioriters = !Prioriters;
    }
    private void Teleport((int, int) pos)
    {
        if (pos == teleports.Item1)
            position = teleports.Item2;
        if (pos == teleports.Item2)
            position = teleports.Item1;
    }
    private void SetDirection()
    {
        switch (currentDirection)
        {
            case directions.SOUTH:
                direction = (1, 0);
                break;
            case directions.NORTH:
                direction = (-1, 0);
                break;
            case directions.EAST:
                direction = (0, 1);
                break;
            case directions.WEST:
                direction = (0, -1);
                break;
        }    

    }
    private void ChangeDirection()
    {
        if (!Prioriters)
            CasualChange();
        else
            ReverseChange();
        SetDirection();
    }
    private void CasualChange()
    {
        switch (currentDirection)
        {
            case directions.SOUTH:
                currentDirection = directions.EAST;
                break;
            case directions.NORTH:
                currentDirection = directions.WEST;
                break;
            case directions.EAST:
                currentDirection = directions.NORTH;
                break;
            case directions.WEST:
                currentDirection = directions.SOUTH;
                break;
        }
    }
    private void ReverseChange()
    {
        switch (currentDirection)
        {
            case directions.SOUTH:
                currentDirection = directions.WEST;
                break;
            case directions.NORTH:
                currentDirection = directions.EAST;
                break;
            case directions.EAST:
                currentDirection = directions.SOUTH;
                break;
            case directions.WEST:
                currentDirection = directions.NORTH;
                break;
        }
    }
    private void Move((int, int) position)
    {
        if (IsGame)
            Console.WriteLine(currentDirection);
        this.position = position;
    }
}
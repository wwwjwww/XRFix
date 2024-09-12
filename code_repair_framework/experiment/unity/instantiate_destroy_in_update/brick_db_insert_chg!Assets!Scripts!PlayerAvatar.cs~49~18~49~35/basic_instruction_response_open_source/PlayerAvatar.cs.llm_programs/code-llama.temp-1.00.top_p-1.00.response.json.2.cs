You've found a bug in the code! It looks like the Update() method is using Instantiate (and Destroy()) in each update. This can be an inefficient! You should use Instantiate(and Destroy()) methods only when you need to add or remove them.

Consider moving instantiation and destruction to other events. You can check your usage for efficiency
}

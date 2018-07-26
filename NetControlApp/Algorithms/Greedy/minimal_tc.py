# Needed only for parsing the heuristics into corresponding parameters
from pyparsing import Word, Forward, Literal, Group, ZeroOrMore, Empty
# Value of +infinity
from sys import maxint as inf
# deque is used only once in the program, and it seems to be
# basicaly just a list which offers better performance in removing and adding elements
# both in the beginning and at the end of it. It should be fully compatible and possible
# to replace it with a simple list.
from collections import deque
# Radom number, not really much to tell here.
from random import random
# As far as I can tell, it is used only once in the program, in the end of it,
# for path.expandvars...
# This refers to the system path, not to be confused with the other variables named path
# which are about the control path in the algorithm.
from os import path
# used only in the initializer of the TCInfo class to create deep copies of four lists.
from copy import deepcopy
# for the command line options, in the ported version of the program we will not need this
import getopt
# For exiting the program, in the ported version of the program, we will again not need this.
import sys

# The random number generator, it is used only in one place in the program.
RAND = lambda x: random()

class TCInfo:
    # Initializes a target control search instance.
    # Parameters:
    # V = the nodes in the graph
    # E = the edges in the graph
    # targets = the list of targets
    # control_path = 
    # controllable = the list of drug target nodes
    # n = number of nodes in the graph
    # pred = predecessors of the nodes in the graph
    #   [1]: []
    #   [2]: [1] 
    #   [3]: [1]
    #   [4]: [1], [3]
    
    def __init__(self, _V, _E, _targets, _control_path, _controllable):
        # save input graph
        self.n = len(_V)
        self.V = deepcopy(_V)
        self.E = deepcopy(_E)
        self.controllable = deepcopy(_controllable)
        # pred[node] = list of predecessors of node
        self.pred = { v : [] for v in self.V }
        for (u, v) in self.E :
            self.pred[v].append(u)
        # control_path[target] = control path from target towards driven
        # * for controlled targets, append None to the end of the path
        # * this is the main data structure, can extract others from this
        # @ elements at the same index are all distinct (if not None)
        if _control_path :
            self.control_path = deepcopy(_control_path)
        else :
            self.control_path = { t : [t] for t in _targets }
        # control_of[target] = driven node that controls target
        self.control_of = self.compute_control_of()
        # controlled_by[driven] = list of targets controlled by driven
        self.controlled_by = self.compute_controlled_by()
        # path_v[node] = { index : target }
        # - target = target whose path contains node
        # - index = index of node in path of target
        # @ each node can appear in at most one path for a given index
        self.path_v = self.compute_path_v()
        # path_e[edge] = { index : target }
        # - target = target whose path contains edge
        # - index = index of edge in path of target (index of first node)
        self.path_e = self.compute_path_e()
        # current step and corresponding targets
        # @ current_targets = path_of[uncontrolled target][current_step]
        # @ path of uncontrolled targets must have length = current_step + 1
        self.current_step = 0
        self.current_targets = self.compute_current_targets()
        # test that everything was initialized properly
        assert self.is_consistent()

    ### compute parts of the internal state ###

    # compute 'control_of' based on 'control_path'
    def compute_control_of(self) :
        return { t : self.control_path[t][-2] for t in self.control_path if self.control_path[t][-1] is None }
    
    # compute 'controlled_by' based on 'control_of'
    def compute_controlled_by(self) :
        ret = {}
        for t, d in self.control_of.items() :
            if d not in ret :
                ret[d] = {t}
            else :
                ret[d].add(t)
        return ret
    
    # compute 'path_v' based on 'control_path'
    def compute_path_v(self) :
        ret = { v : {} for v in self.V }
        for t, path in self.control_path.items() :
            for i in range(len(path)) :
                v = path[i]
                if v is not None :
                    ret[v][i] = t
        return ret

    # compute 'path_e' based on 'control_path'
    def compute_path_e(self) :
        ret = { (v, u) : {} for u, v in self.E }
        for t, path in self.control_path.items() :
            for i in range(len(path) - 1) :
                e = (u, v) = (path[i], path[i + 1])
                if v is not None :
                    ret[e][i] = t
        return ret

    # compute 'current_targets' based on 'control_path' and 'control_of'
    def compute_current_targets(self) :
        return { self.control_path[t][self.current_step] for t in self.control_path if t not in self.control_of and len(self.control_path[t]) == self.current_step + 1 }

    ### test state for consistency ###

    # relies on assertions to test that the data structures are ok
    # run with '-O' to disable assertions
    def is_consistent(self, ignore_current = False) :
        # make sure that graph is consistent across representations
        for v in self.V :
            for u in self.pred[v] :
                assert (u, v) in self.E
        # tests for control_path to make sure it is consistent
        for t, path in self.control_path.items() :
            assert t in self.V, str(t) + ' not in ' + str(self.V)
            for i in range(len(path) - 1) :
                assert (path[i + 1], path[i]) in self.E or path[i + 1] is None, 'edge ' + str((path[i], path[i + 1])) + ' not in E'
        # test for all other data structures
        assert self.control_of == self.compute_control_of()
        assert self.controlled_by == self.compute_controlled_by()
        assert self.path_v == self.compute_path_v()
        assert self.path_e == self.compute_path_e()
        if not ignore_current :
            assert self.current_targets == self.compute_current_targets()
        return True

    ### functions related to the algorithm ###
    
    def is_final(self) :
        return  not self.current_targets
    
    def advance_step(self) :
        assert not self.current_targets
        self.current_step += 1
        self.current_targets = self.compute_current_targets()
    
    # sets a current target as driven
    def set_driven(self, v) :
        assert v in self.current_targets
        assert v not in self.controlled_by
        t = self.path_v[v][self.current_step]
        self.control_path[t].append(None)
        self.control_of[t] = v
        self.controlled_by[v] = {t}
        self.current_targets.remove(v)
        assert self.is_consistent()
    
    # adds an edge for the control of a current target
    def add_edge(self, v, u) :
        assert (u, v) in self.E # valid edge
        assert v in self.current_targets
        t = self.path_v[v][self.current_step]
        self.control_path[t].append(u)
        self.path_v[u][self.current_step + 1] = t
        self.path_e[(v, u)][self.current_step] = t
        self.current_targets.remove(v)
        if u in self.controlled_by :
            self.control_path[t].append(None)
            self.control_of[t] = u
            self.controlled_by[u].add(t)
        assert self.is_consistent()
        
    # removes control of target
    # returns True if target was only controlled by driven
    def remove_control_of(self, t) :
        assert self.control_path[t][-1] is None
        self.control_path[t].pop()
        d = self.control_path[t][-1]
        del self.control_of[t]
        self.controlled_by[d].remove(t)
        if not self.controlled_by[d] :
            del self.controlled_by[d]
            ret = True
        else :
            ret = False
        assert self.is_consistent(ignore_current = True)
        return ret
    
    # removes node from path
    def remove_edge_from(self, t) :
        assert t in self.control_path
        assert len(self.control_path[t]) >= 2
        v = self.control_path[t].pop()
        u = self.control_path[t][-1]
        del self.path_v[v][len(self.control_path[t])]
        del self.path_e[(u, v)][len(self.control_path[t]) - 1]
        assert self.is_consistent(ignore_current = True)

    # remove edges up to a given node
    def shorten_path(self, t, d) :
        assert d in self.control_path[t]
        while d in self.control_path[t][:-1] :
            self.remove_edge_from(t)
        assert self.is_consistent(ignore_current = True)
    
    # set path controlled (by its last node)
    def set_controlled(self, t) :
        assert t in self.control_path
        assert t not in self.control_of
        d = self.control_path[t][-1]
        self.control_of[t] = d
        if d not in self.controlled_by :
            self.controlled_by[d] = set()
        self.controlled_by[d].add(t)
        self.control_path[t].append(None)
        assert self.is_consistent()
    
    # updates control paths to account for later driven nodes
    # returns number of driven nodes 
    def update_control_paths(self, d) :
        if d not in self.controlled_by :
            return 0
        count = 0
        for t in set(self.path_v[d].values()) :
            if t in self.controlled_by[d] :
                # target already controlled by d, just check for repetitions
                if d in self.control_path[t][:-2] :
                    self.remove_control_of(t)
                    self.shorten_path(t, d)
                    self.set_controlled(t)
            elif t in self.control_of :
                # target controlled, but by another driver node
                if self.remove_control_of(t) :
                    count += 1
                self.shorten_path(t, d)
                self.set_controlled(t)
            else :
                # target not controlled before
                self.shorten_path(t, d)
                self.set_controlled(t)
        assert self.is_consistent()
        return count

    ### partition the predecessors of a node, for heuristics ###

    # Define types of predecessor nodes. First approach considers the
    # predecessor nodes independently of the current node, whereas the
    # second one (denoted with @) considers the current node as well.
    #
    # Use concatenation to denote intersection of several sets.
    def compute_hpred(self, nodes) :
        assert set(nodes) <= set(self.current_targets) # don't compute this for already driven nodes
        ret = {}
        for v in nodes :
            ret[v] = { h : set() for h in ['#P', '#N', '@C', '@P', '@N', '@O', '@L', '@A', 'T', 'N', 'D', 'C', 'P', 'O', 'L', 'A', 'K', 'X' ] }
            tv = self.path_v[v][self.current_step]
            # types of nodes w.r.t. (v, u) -- edge
            for u in self.pred[v] :
                # constranints related to paths and control
                if not self.path_e[(v, u)] :
                    ret[v]['@N'].add(u) # New predecessor
                elif any(t in self.control_of for t in self.path_e[(v, u)].values()) :
                    ret[v]['@C'].add(u) # Control path predecessor
                else :
                    ret[v]['@P'].add(u) # Previously seen predecessor
                # constraints related to closing loops
                if tv in self.path_e[(v, u)].values() :
                    ret[v]['@O'].add(u) # follows loop on current path
                elif has_duplicates(self.path_e[(v, u)].values()) :
                    ret[v]['@L'].add(u) # follows loop on another path
                else :
                    ret[v]['@A'].add(u) # acyclic
            # combine for finer grained sets
            ret[v]['@CO'] = ret[v]['@C'] & ret[v]['@O']
            ret[v]['@CL'] = ret[v]['@C'] & ret[v]['@L']
            ret[v]['@CA'] = ret[v]['@C'] & ret[v]['@A']
            ret[v]['@PO'] = ret[v]['@P'] & ret[v]['@O']
            ret[v]['@PL'] = ret[v]['@P'] & ret[v]['@L']
            ret[v]['@PA'] = ret[v]['@P'] & ret[v]['@A']
            # types of nodes w.r.t. u -- predecessor node
            for u in self.pred[v] :
                # no constraint
                ret[v]['T'].add(u)
                # node is controllable
                if u in self.controllable:
                    ret[v]['K'].add(u)
                else:
                    ret[v]['X'].add(u)
                # constraints related to paths and control
                if not self.path_v[u] :
                    ret[v]['N'].add(u) # new node
                elif u in self.controlled_by :
                    ret[v]['D'].add(u) # driven node
                elif any(t in self.control_of for t in self.path_v[u].values()) :
                    ret[v]['C'].add(u) # controlled node
                else :
                    ret[v]['P'].add(u) # previously seen node
                # constraints related to cycles
                if u in self.control_path[tv] :
                    ret[v]['O'].add(u) # node closes cycle on current path
                elif has_duplicates(self.path_v[u].values()) :
                    ret[v]['L'].add(u) # node closes a cycle on another path
                else :
                    ret[v]['A'].add(u) # acyclic
            # combine various constraints for finer grained sets
            ret[v]['DO'] = ret[v]['D'] & ret[v]['O']
            ret[v]['DL'] = ret[v]['D'] & ret[v]['L']
            ret[v]['DA'] = ret[v]['D'] & ret[v]['A']
            ret[v]['CO'] = ret[v]['C'] & ret[v]['O']
            ret[v]['CL'] = ret[v]['C'] & ret[v]['L']
            ret[v]['CA'] = ret[v]['C'] & ret[v]['A']
            ret[v]['PO'] = ret[v]['P'] & ret[v]['O']
            ret[v]['PL'] = ret[v]['P'] & ret[v]['L']
            ret[v]['PA'] = ret[v]['P'] & ret[v]['A']
            # would it be useful to also add intersections of the two types of constraints?
            # types of nodes w.r.t. v -- current node
            if v in self.controllable:
                ret[v]['#K'] = set(self.pred[v])
                ret[v]['#X'] = set()
            else:
                ret[v]['#K'] = set()
                ret[v]['#X'] = set(self.pred[v])
            # combine current node constraints with other constraints
            ret[v]['#XA'] = ret[v]['#X'] & ret[v]['A']
            ret[v]['#XK'] = ret[v]['#X'] & ret[v]['K']
            ret[v]['#XD'] = ret[v]['#X'] & ret[v]['D']
            ret[v]['#XC'] = ret[v]['#X'] & ret[v]['C']
            ret[v]['#XP'] = ret[v]['#X'] & ret[v]['P']
            ret[v]['#KK'] = ret[v]['#K'] & ret[v]['K']
            ret[v]['#KD'] = ret[v]['#K'] & ret[v]['D']
            ret[v]['#KC'] = ret[v]['#K'] & ret[v]['C']
            ret[v]['#KP'] = ret[v]['#K'] & ret[v]['P']
            #TODO this can be remove, did not bring any added value
            if self.path_v[v].values() == [tv]:
                ret[v]['#N'] = set(self.pred[v])
                ret[v]['#P'] = set()
            else:
                ret[v]['#N'] = set()
                ret[v]['#P'] = set(self.pred[v])
            # combine current node constraints with predecessor constraints
            ret[v]['#NA'] = ret[v]['#N'] & ret[v]['A']
        return ret
        
    ### compute maximal matching according to formula for heuristics ###
    
    def get_maximal_matching(self, heuristic) :
        if self.current_step < self.n - 1 :
            return self.compute_maximal_matching(heuristic, self.current_targets)
        else :
            return [], list(self.current_targets)
    
    #@profile
    def compute_maximal_matching(self, heuristics, targets) :
        #@profile
        def compute_maximum(ms, hs) :
            init_matchings, init_hs = compute_maximal(ms)
            # generate bipartite graph
            left = list(free)
            right = unmatched
            adj = { v : [] for v in left }
            for v in right :
                for h in init_hs + hs :
                    for u in ppred[v][h] :
                        if u in free :
                            adj[u].append(v)
            # get maximum matching
            k, left_to_right, right_to_left = max_matching(left, right, adj, initial = init_matchings)
            # update free nodes, unmatched nodes, and matchings
            ret_matchings = []
            ret_hs = init_hs + hs
            for (u, v) in left_to_right.items() :
                if v is not None :
                    ret_matchings.append((u, v))
                    if (u, v) not in init_matchings :
                        free.remove(u)
                        unmatched.remove(v)
            # return matching
            return ret_matchings, ret_hs
        #@profile
        def compute_maximal(mlist) :
            ret_matchings = []
            ret_hs = []
            for (ms, hs) in mlist :
                new_matchings, new_hs = compute_maximum(ms, hs)
                ret_matchings.extend(new_matchings)
                ret_hs.extend(new_hs)
            return ret_matchings, ret_hs
        assert set(targets) <= set(self.V)
        unmatched = list(targets)
        ppred = self.compute_hpred(unmatched)
        # TODO: make sure that this makes sense !!!
        free = { v for v in self.V if self.current_step + 1 not in self.path_v[v] }
        matching = compute_maximal(heuristics)[0]
        is_set = lambda x : len(x) == len(set(x))
        l, r = zip(*matching) if matching else ([], [])
        assert is_set(l) and is_set(r)
        return matching, unmatched

    def display(self) :
        # print the graph
        print 'Graph:'
        print '-', self.n, 'nodes:', self.V
        print '- edges:', self.E
        print
        # print current step info
        print 'Current step:', self.current_step
        print 'Current targets:', self.current_targets
        print
        # print control paths
        print 'Control paths:'
        for t, path in self.control_path.items() :
            print path
        print
        # print control of each target
        print 'Control for each target:'
        for t in self.control_path :
            print t, ':', self.control_of[t] if t in self.control_of else None, ' ',
        print
        # print targets controlled by each driven
        print 'Targets controlled by each driven:'
        for d, controlled in self.controlled_by.items() :
            print d, ':', list(controlled)
        print

def has_duplicates(xs):
    """Check whether a list contains duplicates."""
    return len(set(xs)) != len(xs)

def find_best_control(g_filename, t_filename, h_filename, c_filename, ref_all, ref_extra):
    # V is a list of unique strings (ex. 1, 2, 3, 4)
    # E is a list of strings. To read over it you should read every second element (ex. 1, 2, 1, 3, 1, 4, 3, 4 
    # means that there is an edge from 1 to 2, from 1 to 3, from 1 to 4 and from 3 to 4)
    # Note: Duplicate edges are not removed.
    V, E = load_graph(g_filename, header = None, sep = "\t")
    # Loads the targets, also as a list of strings (ex. 3, 4)
    targets = load_targets(t_filename, sep = None)
    # As it is here, the program crashes if not all of the targets are found. We should change it such that the
    # targets that are not found will simply be ignored.
    targets_not_found = [v for v in targets if v not in V]
    if targets_not_found:
        print "[error]: the following targets were not found in the graph: {}".format(", ".join(str(v) for v in targets_not_found))
        sys.exit(-1)
    # The parameters are load from the corresponding file. The way they are load seems to be a little weird, meaning
    # that if there are multiple parameters in a file, then only the last ones are remembered. It really feels easier just to somehow
    # encode them directly into the program.
    # The text in the filename seems to be only T F 1 (->@CA)(->@PA)(->D)(->CA)(->PA)(->N)(->T)
    # which will return {'cut_to_driven': True, 'heuristics': ([([], ['@CA']), ([], ['@PA']), ([], ['D']), ([], ['CA']), ([], ['PA']), 
    # ([], ['N']), ([], ['T'])], {}), 'repeat': 1, 'cut_non_branching': False, 'name': 'TF1(->@CA)(->@PA)(->D)(->CA)(->PA)(->N)(->T)'}
    params = load_parameters(h_filename)
    # If there are drug-target nodes, then we load them with the same function as the target nodes, otherwise we ignore them.
    if c_filename is None:
        controllable = []
    else:
        controllable = load_targets(c_filename, sep = None)
    # Prints basic details about the graph.
    print "{} nodes, {} edges, {} targets, {} controllable, at least {} extra".format(
            len(V), len(E), len(targets), len(controllable), min_extra_driven(V, E, targets, controllable))
    try:
        best_all = {ref_extra: None} if ref_extra is not None else {}
        best_extra = {ref_all: None} if ref_all is not None else {}
        times_found = {}
        while True:
            # For each iteration, for each parameter set, par is a dictionary
            for par in params:
                # To hard code them it is enough to write here:
                # cut_to_driven = true
                # heuristics =  ([([], ['@CA']), ([], ['@PA']), ([], ['D']), ([], ['CA']), ([], ['PA'])
                # repeat = 1
                # cut_non_branching = false
                # name = TF1(->@CA)(->@PA)(->D)(->CA)(->PA)(->N)(->T) (the same thing as in the file, only without the seprators)
                # And we simply apply the "target_control" using these parameters.
                res = target_control(V, E, targets, controllable=controllable, **par)
                # As far as I can tell, from here on, it simply checks the current solution (res) and sees if it is better
                # than the previosuly best obtained solution.
                count_all = len(res["driven"])
                count_extra = len(set(res["driven"]) - set(controllable))
                better = False
                if ref_all is None or count_all < ref_all:
                    ref_all = count_all
                    if ref_extra is None or count_extra < ref_extra:
                        ref_extra = count_extra
                    best_extra[count_all] = count_extra
                    best_all[count_extra] = count_all
                    times_found[count_all, count_extra] = 1
                    better = True
                elif ref_extra is None or count_extra < ref_extra:
                    ref_extra = count_extra
                    if ref_all is None or count_all < ref_all:
                        ref_all = count_all
                    best_extra[count_all] = count_extra
                    best_all[count_extra] = count_all
                    times_found[count_all, count_extra] = 1
                    better = True
                elif count_all >= ref_all and (best_all[ref_extra] is None or count_all <= best_all[ref_extra]):
                    if count_all not in best_extra or best_extra[count_all] is None or count_extra < best_extra[count_all]:
                        best_extra[count_all] = count_extra
                        if count_extra not in best_all or best_all[count_extra] is None or count_all < best_all[count_extra]:
                            best_all[count_extra] = count_all
                        times_found[count_all, count_extra] = 1
                        better = True
                    elif count_extra == best_extra[count_all]:
                        times_found[count_all, count_extra] += 1
                    else:
                        print "*",
                        continue
                elif count_extra >= ref_extra and (best_extra[ref_all] is None or count_extra <= best_extra[ref_all]):
                    if count_extra not in best_all or best_all[count_extra] is None or count_all < best_all[count_extra]:
                        best_all[count_extra] = count_all
                        if count_all not in best_extra or best_extra[count_all] is None or count_extra < best_extra[count_all]:
                            best_extra[count_all] = count_extra
                        times_found[count_all, count_extra] = 1
                        better = True
                    elif count_all == best_all[count_extra]:
                        times_found[count_all, count_extra] += 1
                    else:
                        print "*",
                        continue
                else:
                    print "*",
                    continue
                if better:
                    print
                    print count_all, count_extra
                else:
                    if count_all == ref_all or count_extra == ref_extra:
                        print "@",
                    else:
                        print "#",
                    if times_found[count_all, count_extra] > 3:
                        continue
                # If we got this far, then the result is better than before.
                # We write in a file named as below the best result obtained so far.
                # compute base for results file name
                filename_base = "{}__{}_{}__{}".format(path.splitext(t_filename)[0], count_all, count_extra, times_found[count_all, count_extra])
                # save driven nodes and control paths
                with open(filename_base + "_details.txt", "w") as outfile:
                    outfile.write(par["name"] + "\n")
                    outfile.write("\n")
                    outfile.writelines(str(t) + "\n" for t in res["driven"])
                    outfile.write("\n")
                    outfile.writelines(" <- ".join(str(u) for u in res["path"][t][:-1]) + "\n" for t in res["path"])
                # tab separated list of driven nodes with the number of targets they control
                with open(filename_base + "_count.txt", "w") as outfile:
                    outfile.write("Driven\tTargets\n")
                    outfile.writelines(str(t) + "\t" + str(len(res["controlled"][t])) + "\n" for t in res["controlled"] if t in controllable)
                    outfile.write("\nExtra\tTargets\n")
                    outfile.writelines(str(t) + "\t" + str(len(res["controlled"][t])) + "\n" for t in res["controlled"] if t not in controllable)
            print ".",
    except KeyboardInterrupt:
        sys.exit(0)

def load_graph(filename, header = 'list', convert = lambda x : x, sep = ' ') :
    with open(filename, 'r') as infile :
        #
        # None of the "header" parts are needed, this part can be completely ignored.
        #
        first_line = infile.readline().strip()
        lines = [line.strip() for line in infile.readlines() if line.strip()]
        v_info = first_line.strip().split(sep)
        if header == 'number' or header == 'guess' and len(v_info) == 1 :
            # read first line as number of nodes
            V = range(int(v_info[0]))
        elif header == 'list' or header == 'guess' and len(v_info) > 2 :
            # read first line as list of nodes
            V = [convert(v) for v in v_info]
        else :
            # there is no header, put the first line with the others
            V = []
            lines.insert(0, first_line)
        # print [line.split() for line in lines if '\t' not in line]
        #
        # Only from here the actual reading begins.
        #
        E = []
        for line in lines:
            xs = line.strip().split(sep)
            if xs == []:
                break
            else:
                E.append((convert(xs[0]), convert(xs[1])))
        #E = [(convert(u), convert(v)) for line in lines if line.strip() for (u, v) in [line.strip().split(sep)[:2]]]
        if V == [] :
            V = set()
            for u, v in E :
                V.add(u)
                V.add(v)
            V = list(V)
        return V, E

def load_targets(filename, sep=' ', convert=lambda x: x):
    with open(filename, 'r') as infile:
        lines = infile.readlines()
        assert len(lines) == 1 or sep == None
        if sep is None:
            targets = [t.strip() for t in lines if t.strip()]
        else:
            targets = lines[0].split(sep)
        return [convert(t) for t in targets]

def load_parameters(filename):
	# We should get rid of the "repeat" parameter, it doesn't seem to do much.
    with open(filename, 'r') as hfile :
        lines = [line.split() for line in hfile if line.strip() and line[0] != "#"]
        params = [{
            'cut_to_driven' : line[0] == 'T',
            'cut_non_branching' : line[1] == 'T',
            'repeat' : int(line[2]),
            'heuristics' : get_heuristics(line[3]),
            'name' : line[0] + line[1] + line[2] + line[3],
        } for line in lines]
        return params

def target_control(V, E, targets, heuristics = None, controllable = [], repeat = 1, cut_to_driven = True, cut_non_branching = False, verbose = False, test = False, **kwd) :
    # We call this function by calling target_control (V, E, targets, controllable, parameters dictionary)
    # count the nodes removed by the cut_paths optimization
    # will report this number in the verbose version
    cut_paths_count = 0
    # count the path reduction caused by non-branching control
    # as well as the driven nodes saved by this optimization
    # report this in verbose mode
    cut_non_branching_paths = 0
    cut_non_branching_nodes = 0

    init_control_path = None

    ### run algorithm ###
    runs = 0
    while runs < repeat :
        state = TCInfo(V, E, targets, init_control_path, controllable)
        while not state.is_final() :
            matched, unmatched = state.get_maximal_matching(heuristics)
            for v in unmatched :
                state.set_driven(v)
            for (u, v) in matched :
                state.add_edge(v, u)
            
            ### OPTIMIZATION: cut paths containing a later found driven node ###
            if cut_to_driven :
                for v in unmatched :
                    cut_paths_count += state.update_control_paths(v)

            state.advance_step()
                
        # test that all is still in order
        assert state.is_consistent()

        ### OPTIMIZATION: reduce paths that don't branch ###
        if cut_non_branching :
            ready = False
            while not ready :
                ready = True
                for d in state.controlled_by.keys() :
                    if d in state.control_path :
                        # d is also a target, can't reduce control paths
                        continue
                    if d not in state.controlled_by :
                        # d was already removed from the driven set
                        continue
                    nexts = { state.control_path[t][-3] for t in state.controlled_by[d] }
                    if len(nexts) == 1 :
                        # next state is same for all paths starting at d
                        ready = False
                        d_new = list(nexts)[0]
                        cut_non_branching_paths += 1
                        for t in list(state.controlled_by[d]) :
                            state.remove_control_of(t)
                            state.shorten_path(t, d_new)
                            state.set_controlled(t)
                        cut_non_branching_nodes += state.update_control_paths(d_new)
            assert state.is_consistent()
        runs += 1
        if verbose :
            print 'driven nodes for run', runs, ':', len(state.controlled_by)
        if runs < repeat :
            # prepare for continuation
            for d in state.controlled_by.keys() :
                if len(state.controlled_by[d]) == 1 :
                    t = list(state.controlled_by[d])[0]
                    state.remove_control_of(t)
                    state.shorten_path(t, t)
            assert state.is_consistent()
            init_control_path = state.control_path

    # We can get rid of this part, it should be mainly for debug purposes.
    if test :
        if is_controllable(V, E, state.controlled_by.keys(), targets) :
            print 'PASS'
        else :
            print 'FAIL'

    if verbose :
        if cut_to_driven :
            print 'driven nodes saved by cutting paths:', cut_paths_count
        if cut_non_branching :
            print 'non-branching paths reduced:', cut_non_branching_paths
            print 'non-branching paths saved nodes:', cut_non_branching_nodes
    return { 
        'driven' : state.controlled_by.keys(), 
        'path' : state.control_path, 
        'controlled': state.controlled_by,
        'control' : state.control_of }

# I think we don't need to port this at all, which would also make us not need anymore the "pyparsing" module.
def get_heuristics(s):
    etype = Word('\@#TNDCPOLAKX')
    sep = Literal('.').suppress()
    ctype = Group(etype + ZeroOrMore(sep + etype) | Empty())
    heur = Forward()
    lparen = Literal('(').suppress()
    rparen = Literal(')').suppress()
    arrow = Literal('->').suppress()
    step = (lparen + heur.setResultsName('h') + arrow + ctype.setResultsName('t') + rparen).addParseAction(lambda toks: (toks.h.asList(), toks.t.asList()))
    heur << Group(ZeroOrMore(step))
    return heur.parseString(s)[0]

def min_extra_driven(V, E, targets, controllable):
    non_controllable_targets = [u for u in targets if u not in controllable]
    adj = {v: [] for v in non_controllable_targets}
    for (u, v) in E:
        if v in adj:
            adj[v].append(u)
    matching, left_to_right, right_to_left = max_matching(non_controllable_targets, V, adj)
    return len(non_controllable_targets) - matching

def max_matching(U, V, Adj, initial = []) :
    
    # breadth first search
    #@profile
    def BFS ():
        Q = deque(u for u in U if Pair_U[u] is None)
        Dist.update({u : 0 if Pair_U[u] is None else inf for u in U})
        Dist[None] = inf
        while Q :
            u = Q.popleft()
            if Dist[u] < Dist[None]:
                for v in Adj[u] :
                    if Dist[Pair_V[v]] == inf :
                        Dist[Pair_V[v]] = Dist[u] + 1
                        Q.append(Pair_V[v])
        return Dist[None] != inf
    
    # depth first search
    #@profile
    def DFS(u) :
        if u is not None:
            for v in Adj[u] :
                if Dist[Pair_V[v]] == Dist[u] + 1 :
                    if DFS(Pair_V[v]) :
                        Pair_V[v] = u
                        Pair_U[u] = v
                        return True
            Dist[u] = inf
            return False
        return True
    # permute adjacency lists for randomization
    for u in Adj:
        Adj[u].sort(key = RAND)
    # the actual algorithm
    Dist = {}
    Pair_U = { u : None for u in U }
    Pair_V = { v : None for v in V }
    # support for starting with an initial matching --> TEST THIS !!!
    for (u, v) in initial :
        Pair_U[u] = v
        Pair_V[v] = u
    matching = len(initial)
    while BFS() :
        for u in U :
            if Pair_U[u] is None :
                if DFS(u) :
                    matching = matching + 1
    return matching, Pair_U, Pair_V

if __name__ == "__main__":
    run_str = "Try 'target_control --help' for more information."
    try:
        opts, args = getopt.getopt(sys.argv[1:], "hg:t:H:C:", ["help", "graph=", "targets=", "heuristics=", "controllable=", "ref-driven=", "ref-uncontrollable="])
    except getopt.GetoptError as err:
        # default behavior is append to existing db
        print err
        print run_str
        sys.exit(0)
    g_filename = None
    t_filename = None
    h_filename = path.expandvars("$TC_PATH/config/heuristics/simple_fast.txt")
    c_filename = None
    ref_all = None
    ref_extra = None
    #
    g_filename = "graph.txt"
    t_filename = "target.txt"
    h_filename = "simple_fast.txt"
    #
    for opt, arg in opts:
        if opt in ("-h", "--help"):
            print __doc__
        if opt in ("-g", "--graph"):
            g_filename = arg
        elif opt in ("-t", "--targets"):
            t_filename = arg
        elif opt in ("-H", "--heuristics"):
            h_filename = arg
        elif opt in ("-C", "--controllable"):
            c_filename = arg
        elif opt == "--ref-driven":
            ref_all = int(arg)
        elif opt == "--ref-uncontrollable":
            ref_extra = int(arg)
        else:
            print "unrecognized option: {}".format(opt)
    if g_filename is None:
        print "Graph file is required."
        print run_str
        sys.exit(0)
    if t_filename is None:
        print "Targets file is required."
        print run_str
        sys.exit(0)
    find_best_control(g_filename, t_filename, h_filename, c_filename, ref_all, ref_extra)